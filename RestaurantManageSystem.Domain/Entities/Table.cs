using RestaurantManageSystem.Domain._Common;
using RestaurantManageSystem.Domain.Enums;

namespace RestaurantManageSystem.Domain.Entities
{
    public class Table : BaseEntity
    {
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public TableStatus Status { get; set; } = TableStatus.Available;
        public string FloorSection { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<Order> Orders { get; set; }

        // Business logic methods
        public void MarkAsOccupied()
        {
            if (Status == TableStatus.Reserved)
                throw new InvalidOperationException("Cannot occupy a reserved table");

            Status = TableStatus.Occupied;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsAvailable()
        {
            Status = TableStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool CanAccommodate(int guests)
        {
            return Capacity >= guests && Status == TableStatus.Available;
        }
    }
}