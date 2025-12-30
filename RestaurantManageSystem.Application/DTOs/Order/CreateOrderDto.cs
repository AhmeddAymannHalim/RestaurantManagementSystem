namespace RestaurantManageSystem.Application.DTOs.Order
{
    public class CreateOrderDto
    {
        public int TableId { get; set; }
        public int UserId { get; set; }
        public string? Notes { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}