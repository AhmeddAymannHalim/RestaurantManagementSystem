using RestaurantManageSystem.Domain.Entities;

namespace RestaurantManageSystem.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
        int? ValidateToken(string token);
    }
}