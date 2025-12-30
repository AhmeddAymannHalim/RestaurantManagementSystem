using RestaurantManageSystem.Application.DTOs.Common;

namespace RestaurantManageSystem.Application.DTOs.Category
{
    public class CategoryDto : BaseDto
    {
        public string CategoryName { get; set; } = null!;
        public string? CategoryNameAr { get; set; }
        public string Description { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public int MenuItemsCount { get; set; }
    }
}