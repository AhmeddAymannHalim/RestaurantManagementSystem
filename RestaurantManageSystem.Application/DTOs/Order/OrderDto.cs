using RestaurantManageSystem.Application.DTOs.Common;

namespace RestaurantManageSystem.Application.DTOs.Order
{
    public class OrderDto : BaseDto
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string OrderNumber { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}