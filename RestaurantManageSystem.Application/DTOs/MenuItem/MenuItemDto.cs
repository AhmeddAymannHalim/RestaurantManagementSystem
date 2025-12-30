using RestaurantManageSystem.Application.DTOs.Common;

namespace RestaurantManageSystem.Application.DTOs.MenuItem
{
    public class MenuItemDto : BaseDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string CategoryNameAr { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int? PreparationTime { get; set; }
    }
}