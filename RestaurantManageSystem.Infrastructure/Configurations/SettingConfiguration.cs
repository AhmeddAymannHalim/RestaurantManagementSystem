using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManageSystem.Domain.Entities;

namespace RestaurantManageSystem.Infrastructure.Configurations
{
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTable("Settings");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Key)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Value)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.Description)
                .HasMaxLength(500);

            builder.Property(s => s.Category)
                .HasMaxLength(50);

            builder.HasIndex(s => s.Key)
                .IsUnique();

            // Seed default email settings
            builder.HasData(
                new Setting
                {
                    Id = 1,
                    Key = "Email.SmtpServer",
                    Value = "smtp.gmail.com",
                    Description = "SMTP Server Address",
                    Category = "Email",
                    CreatedAt = DateTime.UtcNow
                },
                new Setting
                {
                    Id = 2,
                    Key = "Email.SmtpPort",
                    Value = "587",
                    Description = "SMTP Port Number",
                    Category = "Email",
                    CreatedAt = DateTime.UtcNow
                },
                new Setting
                {
                    Id = 3,
                    Key = "Email.FromEmail",
                    Value = "noreply@restaurant.com",
                    Description = "Sender Email Address",
                    Category = "Email",
                    CreatedAt = DateTime.UtcNow
                },
                new Setting
                {
                    Id = 4,
                    Key = "Email.Password",
                    Value = "",
                    Description = "Email Account Password (App Password for Gmail)",
                    Category = "Email",
                    CreatedAt = DateTime.UtcNow
                },
                new Setting
                {
                    Id = 5,
                    Key = "Email.EnableSsl",
                    Value = "true",
                    Description = "Enable SSL for SMTP",
                    Category = "Email",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}