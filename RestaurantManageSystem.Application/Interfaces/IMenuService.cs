using RestaurantManageSystem.Application.DTOs.Category;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.MenuItem;

namespace RestaurantManageSystem.Application.Interfaces
{
    public interface IMenuService
    {
        // Categories
        Task<ResponseDto<List<CategoryDto>>> GetAllCategoriesAsync();
        Task<ResponseDto<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<ResponseDto<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto);
        Task<ResponseDto<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task<ResponseDto<bool>> DeleteCategoryAsync(int id);

        // Menu Items
        Task<ResponseDto<PaginatedResultDto<MenuItemDto>>> GetMenuItemsAsync(int? categoryId, int page, int pageSize);
        Task<ResponseDto<MenuItemDto>> GetMenuItemByIdAsync(int id);
        Task<ResponseDto<MenuItemDto>> CreateMenuItemAsync(CreateMenuItemDto dto);
        Task<ResponseDto<MenuItemDto>> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto);
        Task<ResponseDto<bool>> DeleteMenuItemAsync(int id);
        Task<ResponseDto<bool>> ToggleAvailabilityAsync(int id);
    }
}