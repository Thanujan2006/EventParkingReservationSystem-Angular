using EventParkingReservationSystem.API.IServices;
using System.Net;
using System.Net.Mail;

namespace EventParkingReservationSystem.API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IConfiguration config,
        ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        try
        {
            var host = _config["Smtp:Host"];
            var username = _config["Smtp:Username"];
            var password = _config["Smtp:Password"];
            var fromEmail = _config["Smtp:FromEmail"];

            if (string.IsNullOrWhiteSpace(host))
                throw new Exception("SMTP Host is missing.");

            if (string.IsNullOrWhiteSpace(username))
                throw new Exception("SMTP Username is missing.");

            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("SMTP App Password is missing.");

            using var client = new SmtpClient(
                host,
                _config.GetValue<int>("Smtp:Port"))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    username,
                    password),

                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(
                    fromEmail!,
                    _config["Smtp:FromName"]),

                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mail.To.Add(to);

            await client.SendMailAsync(mail);

            _logger.LogInformation(
                "Verification email sent successfully to {Email}",
                to);
        }
        catch (SmtpException ex)
        {
            _logger.LogError(
                ex,
                "SMTP ERROR sending email to {Email}. Status: {Status}",
                to,
                ex.StatusCode);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "EMAIL ERROR sending email to {Email}",
                to);

            throw;
        }
    }
}