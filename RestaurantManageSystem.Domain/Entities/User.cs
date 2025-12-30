using RestaurantManageSystem.Domain._Common;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Domain.Enums;

namespace RestaurantManageSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; } = null!;
        public string UserNameAr { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string FullName { get; set; } = null!; 
        public UserRoles Role { get; set; }  
        public bool IsActive { get; set; } = true;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}