namespace RestaurantManageSystem.Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string UserName { get; set; } = null!;
        public string UserNameAr { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = "Waiter"; // Default role
    }
}