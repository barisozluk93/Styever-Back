using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using ReportContentManagement.DbContexts;
using ReportContentManagement.Entity;
using ReportContentManagement.Interfaces;
using ReportContentManagement.Model;
using System.Data;

namespace ReportContentManagement.Services
{
    public class ReportContentService : IReportContentService
    {
        private readonly ReportContentManagementContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly MailSettings _mailSettings;

        public ReportContentService(
            ReportContentManagementContext dbContext,
            IConfiguration configuration,
            MailSettings mailSettings)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _mailSettings = mailSettings;
        }

        public async Task<Result<ReportContent>> Save(ReportContent reportContent)
        {
            var result = new Result<ReportContent>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    reportContent.IsDeleted = false;

                    _dbContext.ReportContent.Add(reportContent);
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    await SendMessage(reportContent);

                    result.SetData(reportContent);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }


        public async Task<Result<long>> GetDashboardStats()
        {
            var result = new Result<long>();
            try
            {
                result.SetData(await _dbContext.ReportContent.AsNoTracking().LongCountAsync(x => !x.IsDeleted));
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }
            return result;
        }

        private async Task SendMessage(ReportContent reportContent)
        {
            string message =
                "Yeni içerik şikayeti alındı." + "\n\n" +
                "Ad Soyad: " + reportContent.FullName + "\n" +
                "E-posta: " + reportContent.Email + "\n" +
                "Kullanıcı Id: " + (reportContent.UserId?.ToString() ?? "-") + "\n" +
                "Şikayet Türü: " + reportContent.ComplaintType + "\n" +
                "Şikayet Edilen Sayfa: " + reportContent.ReportedUrl + "\n\n" +
                "Açıklama:" + "\n" + reportContent.Description;

            var emailMessage = new MimeMessage();
            emailMessage.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            emailMessage.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
            emailMessage.To.Add(MailboxAddress.Parse(_mailSettings.Mail));
            emailMessage.Subject = "Styever İçerik Şikayeti";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message };

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(emailMessage);
            smtp.Disconnect(true);
        }
    }
}
