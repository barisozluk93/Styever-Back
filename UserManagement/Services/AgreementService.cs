using Microsoft.EntityFrameworkCore;
using UserManagement.DbContexts;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;

namespace UserManagement.Services
{
    public class AgreementService : IAgreementService
    {
        private readonly UserManagementContext _db;
        public AgreementService(UserManagementContext db) { _db = db; }

        public async Task<Result<List<UserAgreementAcceptance>>> Accept(List<AgreementAcceptanceRequest> requests, string? ipAddress, string? userAgent)
        {
            var result = new Result<List<UserAgreementAcceptance>>();
            if (requests == null || requests.Count == 0 || requests.Any(x => x.UserId <= 0 || string.IsNullOrWhiteSpace(x.AgreementType)))
            {
                result.SetIsSuccess(false); result.SetMessage("Sözleşme onay bilgileri geçersiz."); return result;
            }
            var now = DateTime.UtcNow;
            var rows = requests.Select(x => new UserAgreementAcceptance
            {
                UserId=x.UserId, AgreementType=x.AgreementType.Trim(), Title=x.Title?.Trim() ?? x.AgreementType,
                Version=string.IsNullOrWhiteSpace(x.Version)?"1.0":x.Version.Trim(), Language=string.IsNullOrWhiteSpace(x.Language)?"tr":x.Language.Trim(),
                Context=x.Context?.Trim() ?? string.Empty, DocumentUrl=x.DocumentUrl, ContentSnapshot=x.ContentSnapshot,
                RelatedReference=x.RelatedReference, IpAddress=ipAddress, UserAgent=userAgent, AcceptedDate=now, IsDeleted=false
            }).ToList();
            _db.UserAgreementAcceptances.AddRange(rows); await _db.SaveChangesAsync();
            result.SetData(rows); result.SetMessage("Sözleşme onayları kaydedildi."); return result;
        }
        public async Task<Result<List<UserAgreementAcceptance>>> GetByUser(long userId)
        {
            var result=new Result<List<UserAgreementAcceptance>>();
            result.SetData(await _db.UserAgreementAcceptances.AsNoTracking().Where(x=>x.UserId==userId&&!x.IsDeleted).OrderByDescending(x=>x.AcceptedDate).ToListAsync());
            return result;
        }
    }
}
