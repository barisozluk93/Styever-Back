using ContactUsManagement.DbContexts;
using ContactUsManagement.Entity;
using ContactUsManagement.Interfaces;
using ContactUsManagement.Model;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;

namespace ContactUsManagement.Services
{
    public class ContactUsService : IContactUsService
    {
        private readonly ContactUsManagementContext _dbContext;

        private readonly IConfiguration _configuration;

        private readonly MailSettings _mailSettings;

        public ContactUsService(ContactUsManagementContext dbContext, IConfiguration configuration, MailSettings mailSettings)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _mailSettings = mailSettings;
        }

        public async Task<Result<ContactUs>> Save(ContactUs contactUs)
        {
            var result = new Result<ContactUs>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    contactUs.IsDeleted = false;
                    _dbContext.ContactUs.Add(contactUs);
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                    

                    await SendMessage(contactUs);
                    result.SetData(contactUs);
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

        private async Task SendMessage(ContactUs contactUs)
        {
            string message = contactUs.Message + "\n\n" +
                    contactUs.Fullname + ", " + contactUs.Email;

            var emailMessage = new MimeMessage();
            emailMessage.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            emailMessage.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
            emailMessage.To.Add(MailboxAddress.Parse(_mailSettings.Mail));
            emailMessage.Subject = contactUs.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message };

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(emailMessage);
            smtp.Disconnect(true);
        }

    }
}
