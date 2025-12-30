namespace RestaurantManageSystem.Application.DTOs.Category
{
    public class CreateCategoryDto
    {
        public string CategoryName { get; set; } = null!;
        public string? CategoryNameAr { get; set; }
        public string Description { get; set; } = null!;
        public int DisplayOrder { get; set; } = 0;
    }
}   