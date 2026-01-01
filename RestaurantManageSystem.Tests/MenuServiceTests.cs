using AutoMapper;
using Moq;
using RestaurantManageSystem.Application.DTOs.Category;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.MenuItem;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Application.Mappings;
using RestaurantManageSystem.Application.Services;
using RestaurantManageSystem.Domain.Entities;

namespace RestaurantManageSystem.Tests
{
    public class MenuServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly IMapper _mapper;
        private readonly MenuService _menuService;

        public MenuServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCacheService = new Mock<ICacheService>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
            _menuService = new MenuService(_mockUnitOfWork.Object, _mapper, _mockCacheService.Object);
        }

        #region Category Tests

        [Fact]
        public async Task GetAllCategoriesAsync_WithCachedData_ReturnsCachedCategories()
        {
            // Arrange
            var cachedCategories = new List<CategoryDto>
            {
                new CategoryDto { Id = 1, Name = "Appetizers", Description = "Starters" },
                new CategoryDto { Id = 2, Name = "Main Courses", Description = "Main dishes" }
            };

            _mockCacheService.Setup(c => c.GetAsync<List<CategoryDto>>("all_categories"))
                .ReturnsAsync(cachedCategories);

            // Act
            var result = await _menuService.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotEmpty(result.Data);
            Assert.Equal(2, result.Data.Count);
            _mockUnitOfWork.Verify(u => u.Categories.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_WithoutCachedData_FetchesFromDatabase()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Appetizers", Description = "Starters", IsActive = true },
                new Category { Id = 2, Name = "Main Courses", Description = "Main dishes", IsActive = true }
            };

            _mockCacheService.Setup(c => c.GetAsync<List<CategoryDto>>("all_categories"))
                .ReturnsAsync((List<CategoryDto>)null);

            _mockUnitOfWork.Setup(u => u.Categories.GetAllAsync())
                .ReturnsAsync(categories);

            _mockCacheService.Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<List<CategoryDto>>(),
                It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _menuService.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotEmpty(result.Data);
            Assert.Equal(2, result.Data.Count);
            _mockUnitOfWork.Verify(u => u.Categories.GetAllAsync(), Times.Once);
            _mockCacheService.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<List<CategoryDto>>(),
                It.IsAny<TimeSpan?>()), Times.Once);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_WithValidId_ReturnsCategoryFromCache()
        {
            // Arrange
            var categoryId = 1;
            var cachedCategory = new CategoryDto { Id = 1, Name = "Appetizers", Description = "Starters" };

            _mockCacheService.Setup(c => c.GetAsync<CategoryDto>($"category_{categoryId}"))
                .ReturnsAsync(cachedCategory);

            // Act
            var result = await _menuService.GetCategoryByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Appetizers", result.Data.Name);
            _mockUnitOfWork.Verify(u => u.Categories.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_WithInvalidId_ReturnsFailure()
        {
            // Arrange
            var categoryId = 999;
            _mockCacheService.Setup(c => c.GetAsync<CategoryDto>(It.IsAny<string>()))
                .ReturnsAsync((CategoryDto)null);

            _mockUnitOfWork.Setup(u => u.Categories.GetByIdAsync(categoryId))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _menuService.GetCategoryByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task CreateCategoryAsync_WithValidData_CreatesAndInvalidatesCache()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto { Name = "Desserts", Description = "Sweet treats" };

            _mockUnitOfWork.Setup(u => u.Categories.AddAsync(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mockCacheService.Setup(c => c.RemoveAsync("all_categories"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _menuService.CreateCategoryAsync(createCategoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Desserts", result.Data.Name);
            _mockCacheService.Verify(c => c.RemoveAsync("all_categories"), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_WithValidData_UpdatesAndInvalidatesCache()
        {
            // Arrange
            var categoryId = 1;
            var updateCategoryDto = new UpdateCategoryDto { Name = "Updated Category", Description = "Updated description" };
            var category = new Category { Id = categoryId, Name = "Old Category", Description = "Old description", IsActive = true };

            _mockUnitOfWork.Setup(u => u.Categories.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            _mockUnitOfWork.Setup(u => u.Categories.UpdateAsync(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mockCacheService.Setup(c => c.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _menuService.UpdateCategoryAsync(categoryId, updateCategoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            _mockCacheService.Verify(c => c.RemoveAsync($"category_{categoryId}"), Times.Once);
            _mockCacheService.Verify(c => c.RemoveAsync("all_categories"), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WithValidId_DeletesAndInvalidatesCache()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Category to delete", IsActive = true };

            _mockUnitOfWork.Setup(u => u.Categories.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            _mockUnitOfWork.Setup(u => u.MenuItems.FindAsync(It.IsAny<Func<MenuItem, bool>>()))
                .ReturnsAsync(new List<MenuItem>());

            _mockUnitOfWork.Setup(u => u.Categories.DeleteAsync(categoryId))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mockCacheService.Setup(c => c.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _menuService.DeleteCategoryAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            _mockCacheService.Verify(c => c.RemoveAsync($"category_{categoryId}"), Times.Once);
            _mockCacheService.Verify(c => c.RemoveAsync("all_categories"), Times.Once);
        }

        #endregion

        #region Menu Item Tests

        [Fact]
        public async Task GetMenuItemsAsync_WithCachedData_ReturnsCachedResults()
        {
            // Arrange
            var cachedResult = new PaginatedResultDto<MenuItemDto>
            {
                Items = new List<MenuItemDto>
                {
                    new MenuItemDto { Id = 1, Name = "Pasta", Price = 50 }
                },
                Page = 1,
                PageSize = 10,
                TotalRecords = 1
            };

            _mockCacheService.Setup(c => c.GetAsync<PaginatedResultDto<MenuItemDto>>(It.IsAny<string>()))
                .ReturnsAsync(cachedResult);

            // Act
            var result = await _menuService.GetMenuItemsAsync(null, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotEmpty(result.Data.Items);
            Assert.Equal(1, result.Data.Items.Count);
            _mockUnitOfWork.Verify(u => u.MenuItems.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetMenuItemByIdAsync_WithValidId_ReturnsMenuItem()
        {
            // Arrange
            var menuItemId = 1;
            var menuItem = new MenuItem
            {
                Id = menuItemId,
                Name = "Pasta",
                Description = "Delicious pasta",
                Price = 50,
                CategoryId = 1,
                IsAvailable = true,
                IsActive = true
            };

            _mockCacheService.Setup(c => c.GetAsync<MenuItemDto>(It.IsAny<string>()))
                .ReturnsAsync((MenuItemDto)null);

            _mockUnitOfWork.Setup(u => u.MenuItems.GetByIdAsync(menuItemId))
                .ReturnsAsync(menuItem);

            _mockCacheService.Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<MenuItemDto>(),
                It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _menuService.GetMenuItemByIdAsync(menuItemId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Pasta", result.Data.Name);
            Assert.Equal(50, result.Data.Price);
        }

        [Fact]
        public async Task CreateMenuItemAsync_WithValidData_CreatesMenuItem()
        {
            // Arrange
            var createMenuItemDto = new CreateMenuItemDto
            {
                Name = "Pizza",
                Description = "Delicious pizza",
                Price = 75,
                CategoryId = 1,
                ImageUrl = "pizza.jpg"
            };

            var category = new Category { Id = 1, Name = "Italian", IsActive = true };

            _mockUnitOfWork.Setup(u => u.Categories.GetByIdAsync(1))
                .ReturnsAsync(category);

            _mockUnitOfWork.Setup(u => u.MenuItems.AddAsync(It.IsAny<MenuItem>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _menuService.CreateMenuItemAsync(createMenuItemDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Pizza", result.Data.Name);
            Assert.Equal(75, result.Data.Price);
        }

        [Fact]
        public async Task DeleteMenuItemAsync_WithInvalidId_ReturnsFailure()
        {
            // Arrange
            var menuItemId = 999;
            _mockUnitOfWork.Setup(u => u.MenuItems.GetByIdAsync(menuItemId))
                .ReturnsAsync((MenuItem)null);

            // Act
            var result = await _menuService.DeleteMenuItemAsync(menuItemId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.False(result.Data);
        }

        [Fact]
        public async Task ToggleAvailabilityAsync_WithValidId_TogglesAvailability()
        {
            // Arrange
            var menuItemId = 1;
            var menuItem = new MenuItem
            {
                Id = menuItemId,
                Name = "Pasta",
                Price = 50,
                CategoryId = 1,
                IsAvailable = true,
                IsActive = true
            };

            _mockUnitOfWork.Setup(u => u.MenuItems.GetByIdAsync(menuItemId))
                .ReturnsAsync(menuItem);

            _mockUnitOfWork.Setup(u => u.MenuItems.UpdateAsync(It.IsAny<MenuItem>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mockCacheService.Setup(c => c.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _menuService.ToggleAvailabilityAsync(menuItemId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.False(menuItem.IsAvailable);
        }

        #endregion
    }
}
