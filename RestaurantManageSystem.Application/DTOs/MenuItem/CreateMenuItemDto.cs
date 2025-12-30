namespace RestaurantManageSystem.Application.DTOs.MenuItem
{
    public class CreateMenuItemDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int? PreparationTime { get; set; }
    }
}