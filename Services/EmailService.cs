using EventParkingReservationSystem.API.IServices;
using System.Net;
using System.Net.Mail;

namespace EventParkingReservationSystem.API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var host = _config["Smtp:Host"];
        if (string.IsNullOrWhiteSpace(host))
        {
            _logger.LogInformation("DEV EMAIL to {To}: {Subject}\n{Body}", to, subject, body);
            return;
        }

        using var client = new SmtpClient(host, _config.GetValue<int?>("Smtp:Port") ?? 587)
        {
            EnableSsl = _config.GetValue<bool?>("Smtp:EnableSsl") ?? true
        };

        var username = _config["Smtp:Username"];
        if (!string.IsNullOrWhiteSpace(username))
        {
            client.Credentials = new NetworkCredential(username, _config["Smtp:Password"]);
        }

        var fromEmail = _config["Smtp:FromEmail"] ?? "noreply@eventparking.local";
        var fromName = _config["Smtp:FromName"] ?? "Event & Parking Reservation System";

        using var mail = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        mail.To.Add(to);
        await client.SendMailAsync(mail);
    }
}
