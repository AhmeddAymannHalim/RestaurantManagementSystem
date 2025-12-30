namespace RestaurantManageSystem.Application.DTOs.Order
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = null!;
        public string MenuItemNameAr { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string? SpecialRequest { get; set; }
    }
}