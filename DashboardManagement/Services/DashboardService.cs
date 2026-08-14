using System.Net.Http.Headers;
using DashboardManagement.Interfaces;
using DashboardManagement.Model;
using Newtonsoft.Json;

namespace DashboardManagement.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _config;
        public DashboardService(IHttpClientFactory factory, IConfiguration config) { _factory = factory; _config = config; }

        public async Task<Result<DashboardResponse>> Get(DateTime? startDate, DateTime? endDate, string? authorization)
        {
            var result = new Result<DashboardResponse>();
            try
            {
                var endInput = endDate ?? DateTime.Today;
                var startInput = startDate ?? endInput.AddDays(-6);

                // Dashboard filtreleri saat dilimi değil takvim günü taşır.
                // Alt servisler bu günleri Europe/Istanbul sınırlarına göre UTC'ye çevirir.
                var end = DateTime.SpecifyKind(endInput.Date, DateTimeKind.Unspecified);
                var start = DateTime.SpecifyKind(startInput.Date, DateTimeKind.Unspecified);

                if (start > end)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Başlangıç tarihi bitiş tarihinden büyük olamaz.");
                    return result;
                }

                var userBaseUrl = _config["AppSettings:ApiUrl"]?.TrimEnd('/');
                var memoryBaseUrl = _config["AppSettings:ApiUrl"]?.TrimEnd('/');
                if (string.IsNullOrWhiteSpace(userBaseUrl) || string.IsNullOrWhiteSpace(memoryBaseUrl))
                    throw new InvalidOperationException("Dashboard servis adresleri tanımlı değil.");

                var client = _factory.CreateClient();
                if (!string.IsNullOrWhiteSpace(authorization))
                    client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authorization);

                var qs = $"?startDate={Uri.EscapeDataString(start.ToString("yyyy-MM-dd"))}" +
                         $"&endDate={Uri.EscapeDataString(end.ToString("yyyy-MM-dd"))}";
                
                var userTask = client.GetAsync($"{userBaseUrl}/api/User/DashboardStats{qs}");
                var memoryTask = client.GetAsync($"{memoryBaseUrl}/api/Memory/DashboardStats{qs}");
                var reportTask = client.GetAsync($"{userBaseUrl}/api/ReportContent/DashboardStats");
                await Task.WhenAll(userTask, memoryTask, reportTask);

                var userResponse = await userTask;
                var memoryResponse = await memoryTask;
                var reportResponse = await reportTask;
                var userJson = await userResponse.Content.ReadAsStringAsync();
                var memoryJson = await memoryResponse.Content.ReadAsStringAsync();
                var reportJson = await reportResponse.Content.ReadAsStringAsync();

                if (!userResponse.IsSuccessStatusCode || !memoryResponse.IsSuccessStatusCode)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage($"Dashboard alt servis hatası. User: {(int)userResponse.StatusCode}, Memory: {(int)memoryResponse.StatusCode}");
                    return result;
                }

                var ur = JsonConvert.DeserializeObject<Result<UserStats>>(userJson);
                var mr = JsonConvert.DeserializeObject<Result<MemoryStats>>(memoryJson);
                var rr = reportResponse.IsSuccessStatusCode ? JsonConvert.DeserializeObject<Result<long>>(reportJson) : null;
                if (ur?.GetIsSuccess() != true || mr?.GetIsSuccess() != true || ur.GetData() == null || mr.GetData() == null)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Dashboard verileri alınamadı.");
                    return result;
                }

                var u = ur.GetData()!;
                var m = mr.GetData()!;
                result.SetData(new DashboardResponse
                {
                    StartDate = start,
                    EndDate = end,
                    TotalUsers = u.TotalUsers,
                    ActiveMembers = u.ActiveMembers,
                    TotalMemories = m.TotalMemories,
                    PeriodMemories = m.PeriodMemories,
                    OriginUsers = u.OriginUsers,
                    HeartUsers = u.HeartUsers,
                    FamilyUsers = u.FamilyUsers,
                    MembershipRevenue = u.MembershipRevenue,
                    GiftRevenue = u.GiftRevenue,
                    TotalGifts = u.TotalGifts,
                    GiftVoucherUsers = u.GiftVoucherUsers,
                    RegularUsers = u.RegularUsers,
                    ExpiredTrialUsers = u.ExpiredTrialUsers,
                    ExpiredPackageUsers = u.ExpiredPackageUsers,
                    TotalLikes = m.TotalLikes,
                    TotalComments = m.TotalComments,
                    TotalCandles = m.TotalCandles,
                    AverageInteractionsPerMemory = m.AverageInteractionsPerMemory,
                    ReportedContentCount = rr?.GetIsSuccess() == true ? rr.GetData() : 0,
                    Trend = u.Trend,
                    RecentActivities = (u.RecentActivities ?? new List<DashboardRecentActivity>())
                        .Concat(m.RecentActivities ?? new List<DashboardRecentActivity>())
                        .OrderByDescending(x => x.Date)
                        .Take(8)
                        .ToList()
                });
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }
            return result;
        }
    }
}
