namespace RestaurantManageSystem.Application.DTOs.Table
{
    public class UpdateTableDto
    {
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = null!;
        public string FloorSection { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}