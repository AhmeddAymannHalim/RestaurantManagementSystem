using RestaurantManageSystem.Application.DTOs.Auth;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Domain.Enums;

namespace RestaurantManageSystem.Application.Services
{
    public class AuthService(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService) : IAuthService
    {
        public async Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var users = await unitOfWork.Users.FindAsync(u => u.UserName == request.UserName);
                var user = users.FirstOrDefault();

                if (user == null || !user.IsActive)
                    return ResponseDto<LoginResponseDto>.FailureResponse("Invalid username or password");

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return ResponseDto<LoginResponseDto>.FailureResponse("Invalid username or password");

                var token = jwtTokenService.GenerateToken(user);
                var expiresAt = DateTime.UtcNow.AddMinutes(60);

                var response = new LoginResponseDto
                {
                    Token = token,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Role = user.Role.ToString(),
                    Email = user.Email,
                    ExpiresAt = expiresAt
                };

                return ResponseDto<LoginResponseDto>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                return ResponseDto<LoginResponseDto>.FailureResponse($"Login failed: {ex.Message}");
            }
        }

        public async Task<ResponseDto<LoginResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                var existingUsers = await unitOfWork.Users.FindAsync(u => u.UserName == request.UserName);
                if (existingUsers.Any())
                    return ResponseDto<LoginResponseDto>.FailureResponse("Username already exists");

                var existingEmails = await unitOfWork.Users.FindAsync(u => u.Email == request.Email);
                if (existingEmails.Any())
                    return ResponseDto<LoginResponseDto>.FailureResponse("Email already exists");

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new User
                {
                    UserName = request.UserName,
                    UserNameAr = request.UserNameAr,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    FullName = request.FullName,
                    Role = Enum.Parse<UserRoles>(request.Role),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await unitOfWork.Users.AddAsync(user);
                await unitOfWork.SaveChangesAsync();

                var token = jwtTokenService.GenerateToken(user);
                var expiresAt = DateTime.UtcNow.AddHours(12);

                var response = new LoginResponseDto
                {
                    Token = token,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Role = user.Role.ToString(),
                    Email = user.Email,
                    ExpiresAt = expiresAt
                };

                return ResponseDto<LoginResponseDto>.SuccessResponse(response, "Registration successful");
            }
            catch (Exception ex)
            {
                return ResponseDto<LoginResponseDto>.FailureResponse($"Registration failed: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = await unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return ResponseDto<bool>.FailureResponse("User not found");

                if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                    return ResponseDto<bool>.FailureResponse("Invalid old password");

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.Users.UpdateAsync(user);
                await unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.SuccessResponse(true, "Password changed successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailureResponse($"Password change failed: {ex.Message}");
            }
        }
    }
}