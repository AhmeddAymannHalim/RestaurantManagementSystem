using RestaurantManageSystem.Domain._Common;
using RestaurantManageSystem.Domain.Enums;

namespace RestaurantManageSystem.Domain.Entities
{
    public class Order : BaseEntity
    {
        public int TableId { get; set; }
        public int UserId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }

        public Table Table { get; set; }
        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
