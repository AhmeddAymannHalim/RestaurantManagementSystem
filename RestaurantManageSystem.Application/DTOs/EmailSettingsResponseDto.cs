namespace RestaurantManageSystem.Application.DTOs;

public class EmailSettingsResponseDto
{
    public string SmtpServer { get; set; } = string.Empty;
    public string SmtpPort { get; set; } = string.Empty;
    public string EmailFrom { get; set; } = string.Empty;
    public string EnableSsl { get; set; } = string.Empty;
}