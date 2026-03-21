using MemoryManagement.DbContexts;
using MemoryManagement.Entity;
using MemoryManagement.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;


namespace MemoryManagement.BackgroundServices
{
    public class MemoryDailyWorker : BackgroundService
    {
        private readonly ILogger<MemoryDailyWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        public MemoryDailyWorker(ILogger<MemoryDailyWorker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = now.Date.AddHours(13);

                if (now > nextRun)
                    nextRun = nextRun.AddDays(1);

                var delay = nextRun - now;

                _logger.LogInformation($"Bir sonraki çalışma zamanı: {nextRun}");

                try
                {
                    await Task.Delay(delay, stoppingToken);
                    await DoWork(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "UserDailyWorker hata aldı");
                }
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {

            using var scope = _scopeFactory.CreateScope();

            var _dbContext = scope.ServiceProvider
                .GetRequiredService<MemoryManagementContext>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var memories = await _dbContext.Memories.ToListAsync();

                    foreach (var memory in memories)
                    {
                        if (!memory.IsDeleted)
                        {
                            var today = DateTime.UtcNow.Date;

                            if (memory.BirthDate.Month == today.Month && memory.BirthDate.Day == today.Day)
                            {
                                Notification notification = new Notification();
                                notification.Type = "birthdate_anniversary";
                                notification.Title = "Styever";
                                notification.Body = "Bugün en yakın arkadaşınızın doğum günü, " + memory.Name + "' nin anılarını yaşatmaya devam et." ;
                                notification.IsDeleted = false;
                                notification.IsRead = false;
                                notification.UserId = memory.UserId;
                                notification.CreatedAt = DateTime.UtcNow;
                                notification.TargetUrl = "" + memory.Id;

                                await AddNotification(notification);
                            }

                            if (memory.DeathDate.Month == today.Month && memory.DeathDate.Day == today.Day)
                            {
                                Notification notification = new Notification();
                                notification.Type = "deathdate_anniversary";
                                notification.Title = "Styever";
                                notification.Body = "Bugün en yakın arkadaşınızın ölüm yıl dönümü, " + memory.Name + "' nin anılarını yaşatmaya devam et.";
                                notification.IsDeleted = false;
                                notification.IsRead = false;
                                notification.UserId = memory.UserId;
                                notification.CreatedAt = DateTime.UtcNow;
                                notification.TargetUrl = "" + memory.Id;

                                await AddNotification(notification);
                            }
                        }

                        await _dbContext.SaveChangesAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {

                    _logger.LogInformation(ex.Message);
                }
            }

            _logger.LogInformation("🕐 Günlük servis 01:00'da çalıştı");
        }

        private async Task<Notification> AddNotification(Notification notification)
        {
            HttpClient client = new HttpClient();

            var json = JsonConvert.SerializeObject(notification);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(_configuration["AppSettings:ApiUrl"] + "/api/Notification/Save", content);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<Notification> result = JsonConvert.DeserializeObject<Result<Notification>>(responseStr);

                        if (result.GetData() != null)
                        {
                            return result.GetData();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return null;
        }
    }
}
