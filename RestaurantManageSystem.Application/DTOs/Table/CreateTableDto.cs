namespace RestaurantManageSystem.Application.DTOs.Table
{
    public class CreateTableDto
    {
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string FloorSection { get; set; } = null!;
    }
}