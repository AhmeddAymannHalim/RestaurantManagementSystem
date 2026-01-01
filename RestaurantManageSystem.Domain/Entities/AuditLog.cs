using RestaurantManageSystem.Domain._Common;

namespace RestaurantManageSystem.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public int? UserId { get; set; }
        public string Action { get; set; } = null!;
        public string Entity { get; set; } = null!;
        public int? EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }

        public User? User { get; set; }
    }
}
