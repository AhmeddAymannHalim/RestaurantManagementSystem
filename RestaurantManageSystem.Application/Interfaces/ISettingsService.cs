using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.Setting;

namespace RestaurantManageSystem.Application.Interfaces
{
    public interface ISettingsService
    {
        Task<ResponseDto<List<SettingDto>>> GetAllSettingsAsync();
        Task<ResponseDto<List<SettingDto>>> GetSettingsByCategoryAsync(string category);
        Task<ResponseDto<SettingDto>> GetSettingByKeyAsync(string key);
        Task<ResponseDto<SettingDto>> UpdateSettingAsync(string key, UpdateSettingDto dto);
        Task<ResponseDto<EmailSettingsDto>> GetEmailSettingsAsync();
        Task<ResponseDto<EmailSettingsDto>> UpdateEmailSettingsAsync(EmailSettingsDto dto);
    }
}