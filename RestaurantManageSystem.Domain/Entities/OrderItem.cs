using RestaurantManageSystem.Domain._Common;
namespace RestaurantManageSystem.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string? SpecialRequest { get; set; }

        // Navigation properties
        public Order Order { get; set; }
        public MenuItem MenuItem { get; set; }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be positive");

            Quantity = newQuantity;
            Subtotal = UnitPrice * Quantity;
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
