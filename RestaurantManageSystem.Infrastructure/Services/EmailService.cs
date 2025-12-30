using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManageSystem.Application.DTOs;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Infrastructure.Data;
using System.Net;
using System.Net.Mail;

namespace RestaurantManageSystem.Infrastructure.Services;

public class EmailService(AppDbContext context, ILogger<EmailService> logger) : IEmailService
{
    public async Task<bool> SendOtpEmailAsync(string email, string otp)
    {
        try
        {
            var smtpServer = await context.Settings
                .Where(s => s.Key == "Email.SmtpServer" && s.IsActive)
                .Select(s => s.Value)
                .FirstOrDefaultAsync();

            var smtpPortStr = await context.Settings
                .Where(s => s.Key == "Email.SmtpPort" && s.IsActive)
                .Select(s => s.Value)
                .FirstOrDefaultAsync();

            var fromEmail = await context.Settings
                .Where(s => s.Key == "Email.FromEmail" && s.IsActive)
                .Select(s => s.Value)
                .FirstOrDefaultAsync();

            var password = await context.Settings
                .Where(s => s.Key == "Email.Password" && s.IsActive)
                .Select(s => s.Value)
                .FirstOrDefaultAsync();

            var enableSslStr = await context.Settings
                .Where(s => s.Key == "Email.EnableSsl" && s.IsActive)
                .Select(s => s.Value)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(password))
            {
                logger.LogError("Email settings not configured");
                return false;
            }

            var smtpPort = int.Parse(smtpPortStr ?? "587");
            var enableSsl = enableSslStr?.ToLower() == "true";

            logger.LogInformation("Sending email via {SmtpServer}:{SmtpPort} from {FromEmail}", smtpServer, smtpPort, fromEmail);

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = enableSsl,
                Timeout = 30000
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, "Restaurant System"),
                Subject = "Password Reset OTP",
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; background: #f9f9f9;'>
                            <div style='background: linear-gradient(135deg, #667eea, #764ba2); padding: 30px; text-align: center; color: white;'>
                                <h1>🍽️ Restaurant Management System</h1>
                            </div>
                            <div style='background: white; padding: 40px; margin-top: 20px;'>
                                <h2>Password Reset Request</h2>
                                <p>Your OTP code is:</p>
                                <div style='background: #f0f0f0; padding: 20px; text-align: center; margin: 20px 0; border-radius: 8px;'>
                                    <span style='font-size: 36px; font-weight: bold; color: #667eea; letter-spacing: 10px;'>{otp}</span>
                                </div>
                                <p><strong>This code will expire in 10 minutes.</strong></p>
                                <p style='color: #666; font-size: 14px;'>If you didn't request this, please ignore this email.</p>
                            </div>
                        </div>
                    </body>
                    </html>
                ",
                IsBodyHtml = true
            };
            message.To.Add(email);

            await client.SendMailAsync(message);
            logger.LogInformation("OTP sent successfully to {Email}", email);
            return true;
        }
        catch (SmtpException ex)
        {
            logger.LogError(ex, "SMTP Error: {StatusCode}", ex.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Email sending failed");
            return false;
        }
    }

    public async Task<bool> SendTestEmailAsync(string email)
    {
        try
        {
            var smtpServer = await context.Settings
                .Where(s => s.Key == "Email.SmtpServer" && s.IsActive)
                .Select(s => s.Value)
                .FirstOrDefaultAsync();

            var smtpPortStr = await context.Settings
                .Where(s => s.Key == "Email.SmtpPort" && s.IsActive)
                .Select(s => s.Value)
                .FirstOrDefaultAsync();

            var fromEmail = await context.Settings
                .Where(s => s.Key == "Email.FromEmail" && s.IsActive)
                .Select(s => s.Value)
                .FirstOrDefaultAsync();

            var password = await context.Settings
                .Where(s => s.Key == "Email.Password" && s.IsActive)
                .Select(s => s.Value)
                .FirstOrDefaultAsync();

            var enableSslStr = await context.Settings
                .Where(s => s.Key == "Email.EnableSsl" && s.IsActive)
                .Select(s => s.Value)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(password))
            {
                logger.LogError("Email settings not configured");
                return false;
            }

            var smtpPort = int.Parse(smtpPortStr ?? "587");
            var enableSsl = enableSslStr?.ToLower() == "true";

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = enableSsl,
                Timeout = 30000
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, "Restaurant System"),
                Subject = "Test Email",
                Body = "This is a test email. If you receive this, your email settings are working correctly!",
                IsBodyHtml = false
            };
            message.To.Add(email);

            await client.SendMailAsync(message);
            logger.LogInformation("Test email sent successfully to {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Test email failed");
            return false;
        }
    }

    public async Task<EmailSettingsResponseDto?> GetEmailSettingsAsync()
    {
        try
        {
            var settings = await context.Settings
                .Where(s => s.Key.StartsWith("Email.") && s.IsActive)
                .ToListAsync();

            if (settings.Count == 0)
                return null;

            return new EmailSettingsResponseDto
            {
                SmtpServer = settings.FirstOrDefault(s => s.Key == "Email.SmtpServer")?.Value ?? string.Empty,
                SmtpPort = settings.FirstOrDefault(s => s.Key == "Email.SmtpPort")?.Value ?? string.Empty,
                EmailFrom = settings.FirstOrDefault(s => s.Key == "Email.FromEmail")?.Value ?? string.Empty,
                EnableSsl = settings.FirstOrDefault(s => s.Key == "Email.EnableSsl")?.Value ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get email settings");
            return null;
        }
    }

    public async Task<bool> UpdateEmailSettingsAsync(Dictionary<string, string> settings)
    {
        try
        {
            foreach (var kvp in settings)
            {
                var key = $"Email.{kvp.Key}";
                var existingSetting = await context.Settings
                    .FirstOrDefaultAsync(s => s.Key == key);

                if (existingSetting is not null)
                {
                    existingSetting.Value = kvp.Value;
                    existingSetting.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    context.Settings.Add(new Setting
                    {
                        Key = key,
                        Value = kvp.Value,
                        Category = "Email",
                        Description = $"Email setting: {kvp.Key}",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update email settings");
            return false;
        }
    }
}