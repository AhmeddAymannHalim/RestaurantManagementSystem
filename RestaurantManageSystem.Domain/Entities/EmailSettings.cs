// FILE: Domain/Entities/EmailSettings.cs
// LOCATION: RestaurantManageSystem.Domain/Entities/EmailSettings.cs

using System.ComponentModel.DataAnnotations;

namespace RestaurantManageSystem.Domain.Entities
{
    public class EmailSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string SmtpServer { get; set; } = "smtp.gmail.com";

        [Required]
        public int SmtpPort { get; set; } = 587;

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string FromEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Password { get; set; } = string.Empty;

        public bool EnableSSL { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
    }
}