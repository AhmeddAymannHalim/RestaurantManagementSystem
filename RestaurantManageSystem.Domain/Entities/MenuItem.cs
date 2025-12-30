using RestaurantManageSystem.Domain._Common;
using RestaurantManageSystem.Domain.Entities;

namespace RestaurantManageSystem.Domain.Entities 
{
    public class MenuItem : BaseEntity
    {
        public int CategoryId { get; set; } 
        public string Name { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int? PreparationTime { get; set; }

        public Category Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}