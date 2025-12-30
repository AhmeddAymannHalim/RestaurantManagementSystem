namespace RestaurantManageSystem.Application.DTOs.Order
{
    public class CreateOrderItemDto
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public string? SpecialRequest { get; set; }
    }
}