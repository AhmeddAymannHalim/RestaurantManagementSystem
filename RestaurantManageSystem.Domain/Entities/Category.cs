using RestaurantManageSystem.Domain._Common;
using RestaurantManageSystem.Domain.Entities;

namespace RestaurantManageSystem.Domain.Entities  // ← Fixed namespace!
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; } = null!;
        public string? CategoryNameAr { get; set; }
        public string Description { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<MenuItem> MenuItems { get; set; }
    }
}