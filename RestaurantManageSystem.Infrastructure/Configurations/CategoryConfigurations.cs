using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManageSystem.Domain.Entities;

namespace RestaurantManageSystem.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.CategoryName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.CategoryNameAr)
                .HasMaxLength(100);  

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.Property(e => e.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasMany(e => e.MenuItems)
                .WithOne(m => m.Category)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}