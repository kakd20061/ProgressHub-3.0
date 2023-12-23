using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Crypto.Macs;
using ProgressHubApi.Models.Mail;

namespace ProgressHubApi.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequestModel mailRequest);
    }

    public class MailService : IMailService
    {
        private readonly MailSettingsModel _mailSettings;
        public MailService(IOptions<MailSettingsModel> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequestModel mailRequest)
        {
            var email = new MimeMessage();
            if (Environment.GetEnvironmentVariable("SMTP") != null)
            {
                email.Sender = MailboxAddress.Parse(Environment.GetEnvironmentVariable("SMTP")!);
            }
            else
            {
                email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            }
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 465, true);
            if(Environment.GetEnvironmentVariable("SMTP") != null)
            {
                smtp.Authenticate(Environment.GetEnvironmentVariable("SMTP")!, Environment.GetEnvironmentVariable("PASSWORD")!);

            }else
            {
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            }
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}

