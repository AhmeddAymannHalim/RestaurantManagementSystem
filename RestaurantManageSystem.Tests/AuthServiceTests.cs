using Moq;
using RestaurantManageSystem.Application.DTOs.Auth;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Application.Services;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Domain.Enums;

namespace RestaurantManageSystem.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IJwtTokenService> _mockJwtTokenService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockJwtTokenService = new Mock<IJwtTokenService>();
            _mockEmailService = new Mock<IEmailService>();
            _authService = new AuthService(_mockUnitOfWork.Object, _mockJwtTokenService.Object, _mockEmailService.Object);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccessResponse()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                UserName = "testuser",
                Password = "Password123"
            };

            var user = new User
            {
                Id = 1,
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                FullName = "Test User",
                Role = UserRoles.Admin,
                IsActive = true
            };

            _mockUnitOfWork.Setup(u => u.Users.FindAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(new List<User> { user });

            _mockJwtTokenService.Setup(j => j.GenerateToken(It.IsAny<User>()))
                .Returns("test-token");

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("test-token", result.Data.Token);
            Assert.Equal("testuser", result.Data.UserName);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ReturnsFailureResponse()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                UserName = "testuser",
                Password = "WrongPassword"
            };

            var user = new User
            {
                Id = 1,
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                FullName = "Test User",
                Role = UserRoles.Admin,
                IsActive = true
            };

            _mockUnitOfWork.Setup(u => u.Users.FindAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(new List<User> { user });

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsSuccessResponse()
        {
            // Arrange
            var registerRequest = new RegisterRequestDto
            {
                UserName = "newuser",
                UserNameAr = "?????? ????",
                Email = "newuser@example.com",
                Password = "Password123",
                FullName = "New User",
                Role = "Waiter"
            };

            _mockUnitOfWork.Setup(u => u.Users.FindAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(new List<User>());

            _mockJwtTokenService.Setup(j => j.GenerateToken(It.IsAny<User>()))
                .Returns("test-token");

            // Act
            var result = await _authService.RegisterAsync(registerRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ForgotPasswordAsync_WithValidEmail_ReturnsSuccessResponse()
        {
            // Arrange
            var forgotPasswordRequest = new ForgotPasswordRequestDto
            {
                Email = "test@example.com"
            };

            var user = new User
            {
                Id = 1,
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                FullName = "Test User",
                Role = UserRoles.Admin,
                IsActive = true
            };

            _mockUnitOfWork.Setup(u => u.Users.FindAsync(It.IsAny<Func<User, bool>>()))
                .ReturnsAsync(new List<User> { user });

            _mockEmailService.Setup(e => e.SendOtpEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.ForgotPasswordAsync(forgotPasswordRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ChangePasswordAsync_WithValidOldPassword_ReturnsSuccessResponse()
        {
            // Arrange
            var oldPassword = "OldPassword123";
            var newPassword = "NewPassword123";
            var userId = 1;

            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(oldPassword),
                FullName = "Test User",
                Role = UserRoles.Admin,
                IsActive = true
            };

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.ChangePasswordAsync(userId, oldPassword, newPassword);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }
    }
}
