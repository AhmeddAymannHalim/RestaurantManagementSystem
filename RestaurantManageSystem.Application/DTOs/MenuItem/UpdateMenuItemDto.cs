namespace RestaurantManageSystem.Application.DTOs.MenuItem
{
    public class UpdateMenuItemDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int? PreparationTime { get; set; }
    }
}