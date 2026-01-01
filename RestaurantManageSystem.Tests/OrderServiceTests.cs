using AutoMapper;
using Moq;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.Order;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Application.Mappings;
using RestaurantManageSystem.Application.Services;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Domain.Enums;

namespace RestaurantManageSystem.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly IMapper _mapper;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            _orderService = new OrderService(_mockUnitOfWork.Object, _mapper);
        }

        [Fact]
        public async Task GetOrderByIdAsync_WithValidId_ReturnsOrder()
        {
            // Arrange
            var orderId = 1;
            var order = new Order
            {
                Id = orderId,
                OrderNumber = "ORD-20250101-1234",
                TableId = 1,
                UserId = 1,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                Subtotal = 100,
                Tax = 14,
                TotalAmount = 114,
                OrderItems = new List<OrderItem>()
            };

            _mockUnitOfWork.Setup(u => u.Orders.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(orderId, result.Data.Id);
        }

        [Fact]
        public async Task GetOrderByIdAsync_WithInvalidId_ReturnsFailure()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Orders.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _orderService.GetOrderByIdAsync(999);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task CreateOrderAsync_WithValidData_ReturnsCreatedOrder()
        {
            // Arrange
            var createOrderDto = new CreateOrderDto
            {
                TableId = 1,
                UserId = 1,
                Notes = "Test order",
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        MenuItemId = 1,
                        Quantity = 2,
                        SpecialRequest = "No onions"
                    }
                }
            };

            var table = new Table
            {
                Id = 1,
                TableNumber = 1,
                Capacity = 4,
                Status = TableStatus.Available
            };

            var user = new User
            {
                Id = 1,
                UserName = "waiter1",
                Email = "waiter@example.com",
                PasswordHash = "hash",
                FullName = "Waiter One",
                Role = UserRoles.Waiter,
                IsActive = true
            };

            var menuItem = new MenuItem
            {
                Id = 1,
                Name = "Pasta",
                Description = "Delicious pasta",
                Price = 50,
                CategoryId = 1,
                IsAvailable = true
            };

            _mockUnitOfWork.Setup(u => u.Tables.GetByIdAsync(1))
                .ReturnsAsync(table);

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(1))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(u => u.MenuItems.GetByIdAsync(1))
                .ReturnsAsync(menuItem);

            // Act
            var result = await _orderService.CreateOrderAsync(createOrderDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.OrderNumber);
            Assert.Equal(OrderStatus.Pending, result.Data.Status);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_WithValidTransition_ReturnsUpdatedOrder()
        {
            // Arrange
            var orderId = 1;
            var updateOrderStatusDto = new UpdateOrderStatusDto
            {
                Status = "Preparing"
            };

            var order = new Order
            {
                Id = orderId,
                OrderNumber = "ORD-20250101-1234",
                TableId = 1,
                UserId = 1,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                Subtotal = 100,
                Tax = 14,
                TotalAmount = 114,
                OrderItems = new List<OrderItem>()
            };

            _mockUnitOfWork.Setup(u => u.Orders.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            // Act
            var result = await _orderService.UpdateOrderStatusAsync(orderId, updateOrderStatusDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(OrderStatus.Preparing, result.Data.Status);
        }

        [Fact]
        public async Task CancelOrderAsync_WithValidOrder_ReturnsCancelledOrder()
        {
            // Arrange
            var orderId = 1;
            var table = new Table
            {
                Id = 1,
                TableNumber = 1,
                Capacity = 4,
                Status = TableStatus.Occupied
            };

            var order = new Order
            {
                Id = orderId,
                OrderNumber = "ORD-20250101-1234",
                TableId = 1,
                UserId = 1,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                Subtotal = 100,
                Tax = 14,
                TotalAmount = 114,
                OrderItems = new List<OrderItem>()
            };

            _mockUnitOfWork.Setup(u => u.Orders.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            _mockUnitOfWork.Setup(u => u.Tables.GetByIdAsync(1))
                .ReturnsAsync(table);

            // Act
            var result = await _orderService.CancelOrderAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }
    }
}
