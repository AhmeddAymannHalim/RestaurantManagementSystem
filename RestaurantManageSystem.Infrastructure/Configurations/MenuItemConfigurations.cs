using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManageSystem.Domain.Entities;

namespace RestaurantManageSystem.Infrastructure.Configurations
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.ToTable("MenuItems");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.NameAr)  // ← ADDED!
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            builder.Property(e => e.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(e => e.PreparationTime)
                .HasDefaultValue(null);

            builder.HasIndex(e => e.CategoryId)
                .HasDatabaseName("IX_MenuItems_CategoryId");

            builder.HasIndex(e => e.IsAvailable)
                .HasDatabaseName("IX_MenuItems_IsAvailable");

            // Relationships
            builder.HasOne(e => e.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.OrderItems)
                .WithOne(oi => oi.MenuItem)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);

            
        }
    }
}