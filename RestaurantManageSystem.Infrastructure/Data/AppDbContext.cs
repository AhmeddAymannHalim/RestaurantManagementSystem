// FILE: Infrastructure/Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using RestaurantManageSystem.Domain.Entities;
using System.Reflection;

namespace RestaurantManageSystem.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<EmailSettings> EmailSettings { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // EmailSettings configuration
            modelBuilder.Entity<EmailSettings>(entity =>
            {
                entity.ToTable("EmailSettings");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.SmtpServer)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FromEmail)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("PasswordResetTokens");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.ExpiresAt)
                    .IsRequired();

                entity.Property(e => e.IsUsed)
                    .IsRequired()
                    .HasDefaultValue(false);

        
            });
        }
    }
}