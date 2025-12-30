using AutoMapper;
using RestaurantManageSystem.Application.DTOs.Category;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.MenuItem;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;

namespace RestaurantManageSystem.Application.Services
{
    public class MenuService(IUnitOfWork unitOfWork, IMapper mapper) : IMenuService
    {
        #region Categories

        public async Task<ResponseDto<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await unitOfWork.Categories.GetAllAsync();
                var categoryDtos = mapper.Map<List<CategoryDto>>(categories);
                return ResponseDto<List<CategoryDto>>.SuccessResponse(categoryDtos);
            }
            catch (Exception ex)
            {
                return ResponseDto<List<CategoryDto>>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                    return ResponseDto<CategoryDto>.FailureResponse("Category not found");

                var categoryDto = mapper.Map<CategoryDto>(category);
                return ResponseDto<CategoryDto>.SuccessResponse(categoryDto);
            }
            catch (Exception ex)
            {
                return ResponseDto<CategoryDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            try
            {
                var category = mapper.Map<Category>(dto);
                category.CreatedAt = DateTime.UtcNow;
                category.IsActive = true;

                await unitOfWork.Categories.AddAsync(category);
                await unitOfWork.SaveChangesAsync();

                var categoryDto = mapper.Map<CategoryDto>(category);
                return ResponseDto<CategoryDto>.SuccessResponse(categoryDto, "Category created successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<CategoryDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            try
            {
                var category = await unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                    return ResponseDto<CategoryDto>.FailureResponse("Category not found");

                mapper.Map(dto, category);
                category.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.Categories.UpdateAsync(category);
                await unitOfWork.SaveChangesAsync();

                var categoryDto = mapper.Map<CategoryDto>(category);
                return ResponseDto<CategoryDto>.SuccessResponse(categoryDto, "Category updated successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<CategoryDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                    return ResponseDto<bool>.FailureResponse("Category not found");

                // Check if category has menu items
                var menuItems = await unitOfWork.MenuItems.FindAsync(m => m.CategoryId == id);
                if (menuItems.Any())
                    return ResponseDto<bool>.FailureResponse("Cannot delete category with existing menu items");

                await unitOfWork.Categories.DeleteAsync(id);
                await unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.SuccessResponse(true, "Category deleted successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailureResponse($"Error: {ex.Message}");
            }
        }

        #endregion

        #region Menu Items

        public async Task<ResponseDto<PaginatedResultDto<MenuItemDto>>> GetMenuItemsAsync(int? categoryId, int page, int pageSize)
        {
            try
            {
                IEnumerable<MenuItem> menuItems;

                if (categoryId.HasValue)
                {
                    menuItems = await unitOfWork.MenuItems.FindAsync(m => m.CategoryId == categoryId.Value);
                }
                else
                {
                    menuItems = await unitOfWork.MenuItems.GetAllAsync();
                }

                var totalRecords = menuItems.Count();
                var items = menuItems
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var menuItemDtos = mapper.Map<List<MenuItemDto>>(items);
                var result = PaginatedResultDto<MenuItemDto>.Create(menuItemDtos, page, pageSize, totalRecords);

                return ResponseDto<PaginatedResultDto<MenuItemDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ResponseDto<PaginatedResultDto<MenuItemDto>>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<MenuItemDto>> GetMenuItemByIdAsync(int id)
        {
            try
            {
                var menuItem = await unitOfWork.MenuItems.GetByIdAsync(id);
                if (menuItem == null)
                    return ResponseDto<MenuItemDto>.FailureResponse("Menu item not found");

                var menuItemDto = mapper.Map<MenuItemDto>(menuItem);
                return ResponseDto<MenuItemDto>.SuccessResponse(menuItemDto);
            }
            catch (Exception ex)
            {
                return ResponseDto<MenuItemDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<MenuItemDto>> CreateMenuItemAsync(CreateMenuItemDto dto)
        {
            try
            {
                var category = await unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
                if (category == null)
                    return ResponseDto<MenuItemDto>.FailureResponse("Category not found");

                var menuItem = mapper.Map<MenuItem>(dto);
                menuItem.CreatedAt = DateTime.UtcNow;
                menuItem.IsActive = true;
                menuItem.IsAvailable = true;

                await unitOfWork.MenuItems.AddAsync(menuItem);
                await unitOfWork.SaveChangesAsync();

                var menuItemDto = mapper.Map<MenuItemDto>(menuItem);
                return ResponseDto<MenuItemDto>.SuccessResponse(menuItemDto, "Menu item created successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<MenuItemDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<MenuItemDto>> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto)
        {
            try
            {
                var menuItem = await unitOfWork.MenuItems.GetByIdAsync(id);
                if (menuItem == null)
                    return ResponseDto<MenuItemDto>.FailureResponse("Menu item not found");

                var category = await unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
                if (category == null)
                    return ResponseDto<MenuItemDto>.FailureResponse("Category not found");

                mapper.Map(dto, menuItem);
                menuItem.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.MenuItems.UpdateAsync(menuItem);
                await unitOfWork.SaveChangesAsync();

                var menuItemDto = mapper.Map<MenuItemDto>(menuItem);
                return ResponseDto<MenuItemDto>.SuccessResponse(menuItemDto, "Menu item updated successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<MenuItemDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> DeleteMenuItemAsync(int id)
        {
            try
            {
                var menuItem = await unitOfWork.MenuItems.GetByIdAsync(id);
                if (menuItem == null)
                    return ResponseDto<bool>.FailureResponse("Menu item not found");

                // Check if has orders
                var orderItems = await unitOfWork.OrderItems.FindAsync(oi => oi.MenuItemId == id);

                if (orderItems.Any())
                {
                    return ResponseDto<bool>.FailureResponse(
                        "Cannot delete menu item that has been ordered. Please remove it from all orders first.");
                }

                await unitOfWork.MenuItems.DeleteAsync(id);
                await unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.SuccessResponse(true, "Menu item deleted successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> ToggleAvailabilityAsync(int id)
        {
            try
            {
                var menuItem = await unitOfWork.MenuItems.GetByIdAsync(id);
                if (menuItem == null)
                    return ResponseDto<bool>.FailureResponse("Menu item not found");

                menuItem.IsAvailable = !menuItem.IsAvailable;
                menuItem.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.MenuItems.UpdateAsync(menuItem);
                await unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.SuccessResponse(true,
                    $"Menu item is now {(menuItem.IsAvailable ? "available" : "unavailable")}");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailureResponse($"Error: {ex.Message}");
            }
        }

        #endregion
    }
}