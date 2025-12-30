namespace RestaurantManageSystem.Application.DTOs.Setting
{
    public class SettingDto
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class UpdateSettingDto
    {
        public string Value { get; set; } = string.Empty;
    }

    public class EmailSettingsDto
    {
        public string SmtpServer { get; set; } = string.Empty;
        public string SmtpPort { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }
}