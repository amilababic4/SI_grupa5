using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLib.Core.Interfaces;

namespace SmartLib.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];

            // If SMTP is not configured, log the email to console (development fallback)
            if (string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(senderEmail) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                password == "UNESI_SVOJ_APP_PASSWORD_OVDJE")
            {
                _logger.LogWarning("========== EMAIL (SMTP nije konfigurisan) ==========");
                _logger.LogWarning("To: {ToEmail}", toEmail);
                _logger.LogWarning("Subject: {Subject}", subject);
                _logger.LogWarning("Body: {Body}", message);
                _logger.LogWarning("=====================================================");
                return;
            }

            try
            {
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var senderName = _configuration["EmailSettings:SenderName"] ?? "SmartLib";

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    Timeout = 15000 // 15 seconds max
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                // Use a CancellationToken to enforce timeout on async send
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                await client.SendMailAsync(mailMessage, cts.Token);
                _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
            }
            catch (Exception ex)
            {
                // Don't crash the app — log the error and the email content so the flow still works
                _logger.LogError(ex, "SMTP slanje nije uspjelo za {ToEmail}. Sadržaj maila ispisan ispod.", toEmail);
                _logger.LogWarning("========== EMAIL (slanje nije uspjelo) ==========");
                _logger.LogWarning("To: {ToEmail}", toEmail);
                _logger.LogWarning("Subject: {Subject}", subject);
                _logger.LogWarning("Body: {Body}", message);
                _logger.LogWarning("=================================================");
            }
        }
    }
}
