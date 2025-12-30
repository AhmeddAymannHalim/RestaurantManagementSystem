using RestaurantManageSystem.Application.DTOs;

namespace RestaurantManageSystem.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendOtpEmailAsync(string email, string otp);
    Task<bool> SendTestEmailAsync(string email);
    Task<EmailSettingsResponseDto?> GetEmailSettingsAsync();
    Task<bool> UpdateEmailSettingsAsync(Dictionary<string, string> settings);
}