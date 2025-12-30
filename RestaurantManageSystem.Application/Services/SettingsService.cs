using AutoMapper;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.Setting;
using RestaurantManageSystem.Application.Interfaces;

namespace RestaurantManageSystem.Application.Services
{
    public class SettingsService(IUnitOfWork unitOfWork, IMapper mapper) : ISettingsService
    {
        public async Task<ResponseDto<List<SettingDto>>> GetAllSettingsAsync()
        {
            var settings = await unitOfWork.Settings.GetAllAsync();
            var settingDtos = mapper.Map<List<SettingDto>>(settings);
            return ResponseDto<List<SettingDto>>.SuccessResponse(settingDtos);
        }

        public async Task<ResponseDto<List<SettingDto>>> GetSettingsByCategoryAsync(string category)
        {
            var settings = await unitOfWork.Settings.FindAsync(s => s.Category == category);
            var settingDtos = mapper.Map<List<SettingDto>>(settings.ToList());
            return ResponseDto<List<SettingDto>>.SuccessResponse(settingDtos);
        }

        public async Task<ResponseDto<SettingDto>> GetSettingByKeyAsync(string key)
        {
            var settings = await unitOfWork.Settings.FindAsync(s => s.Key == key);
            var setting = settings.FirstOrDefault();

            if (setting == null)
                return ResponseDto<SettingDto>.FailureResponse("Setting not found");

            var settingDto = mapper.Map<SettingDto>(setting);
            return ResponseDto<SettingDto>.SuccessResponse(settingDto);
        }

        public async Task<ResponseDto<SettingDto>> UpdateSettingAsync(string key, UpdateSettingDto dto)
        {
            var settings = await unitOfWork.Settings.FindAsync(s => s.Key == key);
            var setting = settings.FirstOrDefault();

            if (setting == null)
                return ResponseDto<SettingDto>.FailureResponse("Setting not found");

            setting.Value = dto.Value;
            setting.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Settings.UpdateAsync(setting);
            await unitOfWork.SaveChangesAsync();

            var settingDto = mapper.Map<SettingDto>(setting);
            return ResponseDto<SettingDto>.SuccessResponse(settingDto, "Setting updated successfully");
        }

        public async Task<ResponseDto<EmailSettingsDto>> GetEmailSettingsAsync()
        {
            var emailSettings = await unitOfWork.Settings.FindAsync(s => s.Category == "Email");
            var settings = emailSettings.ToList();

            var emailDto = new EmailSettingsDto
            {
                SmtpServer = settings.FirstOrDefault(s => s.Key == "Email.SmtpServer")?.Value ?? "",
                SmtpPort = settings.FirstOrDefault(s => s.Key == "Email.SmtpPort")?.Value ?? "",
                FromEmail = settings.FirstOrDefault(s => s.Key == "Email.FromEmail")?.Value ?? "",
                Password = settings.FirstOrDefault(s => s.Key == "Email.Password")?.Value ?? "",
                EnableSsl = bool.Parse(settings.FirstOrDefault(s => s.Key == "Email.EnableSsl")?.Value ?? "true")
            };

            return ResponseDto<EmailSettingsDto>.SuccessResponse(emailDto);
        }

        public async Task<ResponseDto<EmailSettingsDto>> UpdateEmailSettingsAsync(EmailSettingsDto dto)
        {
            var emailSettings = await unitOfWork.Settings.FindAsync(s => s.Category == "Email");
            var settings = emailSettings.ToList();

            var smtpServer = settings.FirstOrDefault(s => s.Key == "Email.SmtpServer");
            if (smtpServer != null)
            {
                smtpServer.Value = dto.SmtpServer;
                smtpServer.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.Settings.UpdateAsync(smtpServer);
            }

            var smtpPort = settings.FirstOrDefault(s => s.Key == "Email.SmtpPort");
            if (smtpPort != null)
            {
                smtpPort.Value = dto.SmtpPort;
                smtpPort.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.Settings.UpdateAsync(smtpPort);
            }

            var fromEmail = settings.FirstOrDefault(s => s.Key == "Email.FromEmail");
            if (fromEmail != null)
            {
                fromEmail.Value = dto.FromEmail;
                fromEmail.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.Settings.UpdateAsync(fromEmail);
            }

            var password = settings.FirstOrDefault(s => s.Key == "Email.Password");
            if (password != null)
            {
                password.Value = dto.Password;
                password.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.Settings.UpdateAsync(password);
            }

            var enableSsl = settings.FirstOrDefault(s => s.Key == "Email.EnableSsl");
            if (enableSsl != null)
            {
                enableSsl.Value = dto.EnableSsl.ToString().ToLower();
                enableSsl.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.Settings.UpdateAsync(enableSsl);
            }

            await unitOfWork.SaveChangesAsync();

            return ResponseDto<EmailSettingsDto>.SuccessResponse(dto, "Email settings updated successfully");
        }
    }
}