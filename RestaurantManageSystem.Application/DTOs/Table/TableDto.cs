using RestaurantManageSystem.Application.DTOs.Common;

namespace RestaurantManageSystem.Application.DTOs.Table
{
    public class TableDto : BaseDto
    {
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = null!;
        public string FloorSection { get; set; } = null!;
        public bool IsActive { get; set; }
        public int? CurrentOrderId { get; set; }
        public string? CurrentOrderNumber { get; set; }
    }
}