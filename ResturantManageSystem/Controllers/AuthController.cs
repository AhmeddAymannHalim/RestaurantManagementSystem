using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManageSystem.Application.DTOs.Auth;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Infrastructure.Data;

namespace ResturantManageSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, IUnitOfWork unitOfWork, IEmailService emailService, AppDbContext context) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await authService.LoginAsync(dto);
        if (!result.Success)
            return Unauthorized(result);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        var result = await authService.RegisterAsync(dto);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        try
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Ok(new { success = true, message = "If email exists, OTP has been sent" });
            }

            var otp = new Random().Next(100000, 999999).ToString();
            var expiryTime = DateTime.UtcNow.AddMinutes(10);

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = otp,
                ExpiresAt = expiryTime,
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };
            context.PasswordResetTokens.Add(resetToken);
            await context.SaveChangesAsync();

            var emailSent = await emailService.SendOtpEmailAsync(request.Email, otp);

            if (!emailSent)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to send OTP. Please check email settings."
                });
            }

            return Ok(new { success = true, message = "OTP sent to your email" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred",
                errors = new[] { ex.Message }
            });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        try
        {
            var resetToken = await context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == request.Otp && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);

            if (resetToken == null)
            {
                return BadRequest(new { success = false, message = "Invalid or expired OTP" });
            }

            var user = await context.Users.FindAsync(resetToken.UserId);
            if (user == null)
            {
                return BadRequest(new { success = false, message = "User not found" });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            resetToken.IsUsed = true;

            await context.SaveChangesAsync();

            return Ok(new { success = true, message = "Password reset successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    public class ResetPasswordRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}

public class ForgotPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
}