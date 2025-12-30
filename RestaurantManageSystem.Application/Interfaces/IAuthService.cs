using RestaurantManageSystem.Application.DTOs.Auth;
using RestaurantManageSystem.Application.DTOs.Common;

namespace RestaurantManageSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ResponseDto<LoginResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<ResponseDto<bool>> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }
}