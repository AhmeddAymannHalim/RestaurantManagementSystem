// FILE: API/Controllers/SettingsController.cs
// LOCATION: ResturantManageSystem.API/Controllers/SettingsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManageSystem.Application.DTOs;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.Setting;
using RestaurantManageSystem.Application.Interfaces;
using Asp.Versioning;

namespace ResturantManageSystem.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController(IEmailService emailService) : ControllerBase
    {
        private readonly IEmailService _emailService = emailService;

        /// <summary>
        /// Get email settings
        /// </summary>
        /// <returns>Email configuration settings</returns>
        [HttpGet("email")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDto<EmailSettingsResponseDto>>> GetEmailSettings()
        {
            try
            {
                var settings = await _emailService.GetEmailSettingsAsync();

                if (settings == null)
                {
                    return NotFound(new ResponseDto<EmailSettingsResponseDto>
                    {
                        Success = false,
                        Message = "Email settings not found"
                    });
                }

                return Ok(new ResponseDto<EmailSettingsResponseDto>
                {
                    Success = true,
                    Message = "Email settings retrieved successfully",
                    Data = settings
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<EmailSettingsResponseDto>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Update email settings
        /// </summary>
        /// <param name="dto">Email settings to update</param>
        /// <returns>Update result</returns>
        [HttpPost("email")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDto<bool>>> UpdateEmailSettings([FromBody] EmailSettingsDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new ResponseDto<bool>
                    {
                        Success = false,
                        Message = string.Join(", ", errors)
                    });
                }

                var settings = new Dictionary<string, string>
        {
            { "SmtpServer", dto.SmtpServer },
            { "SmtpPort", dto.SmtpPort },
            { "FromEmail", dto.FromEmail },
            { "Password", dto.Password },
            { "EnableSsl", dto.EnableSsl.ToString() }
        };

                var success = await _emailService.UpdateEmailSettingsAsync(settings);

                if (!success)
                {
                    return BadRequest(new ResponseDto<bool>
                    {
                        Success = false,
                        Message = "Failed to update email settings"
                    });
                }

                return Ok(new ResponseDto<bool>
                {
                    Success = true,
                    Message = "Email settings updated successfully!",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<bool>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Send a test email to verify settings
        /// </summary>
        /// <param name="dto">Test email address</param>
        /// <returns>Test result</returns>
        [HttpPost("email/test")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDto<bool>>> TestEmailSettings([FromBody] TestEmailDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Email))
                {
                    return BadRequest(new ResponseDto<bool>
                    {
                        Success = false,
                        Message = "Email address is required"
                    });
                }

                var success = await _emailService.SendTestEmailAsync(dto.Email);

                if (!success)
                {
                    return BadRequest(new ResponseDto<bool>
                    {
                        Success = false,
                        Message = "Failed to send test email"
                    });
                }

                return Ok(new ResponseDto<bool>
                {
                    Success = true,
                    Message = "Test email sent successfully!",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<bool>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                });
            }
        }
    }

    public class TestEmailDto
    {
        public string Email { get; set; } = string.Empty;
    }
}