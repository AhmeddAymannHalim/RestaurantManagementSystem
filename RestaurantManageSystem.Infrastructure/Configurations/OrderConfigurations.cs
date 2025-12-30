using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManageSystem.Domain.Entities;

namespace RestaurantManageSystem.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.OrderNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(e => e.OrderNumber)
                .IsUnique();

            builder.Property(e => e.OrderDate)
                .IsRequired();

            builder.Property(e => e.Status)
                .IsRequired();

            builder.Property(e => e.Subtotal)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(e => e.Tax)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(e => e.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)")  // ← FIXED!
                .HasDefaultValue(0);

            builder.Property(e => e.Notes)
                .HasMaxLength(500);

            builder.HasIndex(e => e.OrderDate)
                .HasDatabaseName("IX_Orders_OrderDate");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Orders_Status");

            builder.HasIndex(e => e.TableId)
                .HasDatabaseName("IX_Orders_TableId");

            builder.HasIndex(e => new { e.OrderDate, e.Status })
                .HasDatabaseName("IX_Orders_OrderDate_Status");

            // Relationships
            builder.HasOne(e => e.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(e => e.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}