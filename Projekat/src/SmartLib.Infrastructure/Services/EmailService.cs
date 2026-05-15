using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLib.Core.Interfaces;

namespace SmartLib.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private static readonly HttpClient _httpClient = new();

        public EmailService(
            IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            // Strategy 1: Try Brevo HTTP API (works on Render and allows sending to any email)
            var brevoApiKey = _configuration["EmailSettings:BrevoApiKey"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "theofficialsmartlibrary@gmail.com";
            var senderName = _configuration["EmailSettings:SenderName"] ?? "SmartLib";

            if (!string.IsNullOrWhiteSpace(brevoApiKey))
            {
                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
                    request.Headers.Add("api-key", brevoApiKey);

                    var payload = new
                    {
                        sender = new { name = senderName, email = senderEmail },
                        to = new[] { new { email = toEmail } },
                        subject = subject,
                        htmlContent = message
                    };

                    request.Content = new StringContent(
                        JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                    var response = await _httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Email sent via Brevo to {ToEmail}", toEmail);
                        return;
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Brevo API returned {StatusCode}: {Body}", response.StatusCode, responseBody);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Brevo API failed for {ToEmail}, falling back to SMTP", toEmail);
                }
            }

            // Strategy 2: Try SMTP (works locally)
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];

            if (!string.IsNullOrWhiteSpace(smtpServer) &&
                !string.IsNullOrWhiteSpace(username) &&
                !string.IsNullOrWhiteSpace(password) &&
                password != "UNESI_SVOJ_APP_PASSWORD_OVDJE")
            {
                try
                {
                    var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");

                    using var smtpClient = new SmtpClient(smtpServer, smtpPort)
                    {
                        Credentials = new NetworkCredential(username, password),
                        EnableSsl = true,
                        Timeout = 15000
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, senderName),
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(toEmail);

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                    await smtpClient.SendMailAsync(mailMessage, cts.Token);
                    _logger.LogInformation("Email sent via SMTP to {ToEmail}", toEmail);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "SMTP failed for {ToEmail}", toEmail);
                }
            }

            // Strategy 3: Log email content (fallback for development/debugging)
            _logger.LogWarning("========== EMAIL (nijedna metoda slanja nije uspjela) ==========");
            _logger.LogWarning("To: {ToEmail}", toEmail);
            _logger.LogWarning("Subject: {Subject}", subject);
            _logger.LogWarning("Body: {Body}", message);
            _logger.LogWarning("================================================================");
        }
    }
}
