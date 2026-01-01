using AutoMapper;
using RestaurantManageSystem.Application.DTOs.Category;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.MenuItem;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;

namespace RestaurantManageSystem.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<MenuItem> _menuItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MenuService(
            IRepository<Category> categoryRepository,
            IRepository<MenuItem> menuItemRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _menuItemRepository = menuItemRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Categories

        public async Task<ResponseDto<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
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
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                    return ResponseDto<CategoryDto>.FailureResponse("Category not found");

                var categoryDto = _mapper.Map<CategoryDto>(category);
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
                var category = _mapper.Map<Category>(dto);
                await _categoryRepository.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var createdCategory = await _categoryRepository.GetByIdAsync(category.Id);
                var categoryDto = _mapper.Map<CategoryDto>(createdCategory);

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
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                    return ResponseDto<CategoryDto>.FailureResponse("Category not found");

                _mapper.Map(dto, category);
                category.Id = id;

                await _categoryRepository.UpdateAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var updatedCategory = await _categoryRepository.GetByIdAsync(id);
                var categoryDto = _mapper.Map<CategoryDto>(updatedCategory);

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
                await _categoryRepository.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

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
                IEnumerable<MenuItem> items;

                if (categoryId.HasValue)
                {
                    items = await _menuItemRepository.FindAsync(m => m.CategoryId == categoryId.Value);
                }
                else
                {
                    items = await _menuItemRepository.GetAllAsync();
                }

                var totalRecords = items.Count();
                var paginatedItems = items
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var itemDtos = _mapper.Map<List<MenuItemDto>>(paginatedItems);
                var result = PaginatedResultDto<MenuItemDto>.Create(itemDtos, page, pageSize, totalRecords);

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
                var item = await _menuItemRepository.GetByIdAsync(id);
                if (item == null)
                    return ResponseDto<MenuItemDto>.FailureResponse("Menu item not found");

                var itemDto = _mapper.Map<MenuItemDto>(item);
                return ResponseDto<MenuItemDto>.SuccessResponse(itemDto);
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
                var item = _mapper.Map<MenuItem>(dto);
                await _menuItemRepository.AddAsync(item);
                await _unitOfWork.SaveChangesAsync();

                var createdItem = await _menuItemRepository.GetByIdAsync(item.Id);
                var itemDto = _mapper.Map<MenuItemDto>(createdItem);

                return ResponseDto<MenuItemDto>.SuccessResponse(itemDto, "Menu item created successfully");
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
                var item = await _menuItemRepository.GetByIdAsync(id);
                if (item == null)
                    return ResponseDto<MenuItemDto>.FailureResponse("Menu item not found");

                _mapper.Map(dto, item);
                item.Id = id;

                await _menuItemRepository.UpdateAsync(item);
                await _unitOfWork.SaveChangesAsync();

                var updatedItem = await _menuItemRepository.GetByIdAsync(id);
                var itemDto = _mapper.Map<MenuItemDto>(updatedItem);

                return ResponseDto<MenuItemDto>.SuccessResponse(itemDto, "Menu item updated successfully");
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
                await _menuItemRepository.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.SuccessResponse(true, "Menu item deleted successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailureResponse($"Error: {ex.Message}");
            }
        }

        // ✅ FIXED: Now returns Task<ResponseDto<bool>>
        public async Task<ResponseDto<bool>> ToggleAvailabilityAsync(int id)
        {
            try
            {
                var item = await _menuItemRepository.GetByIdAsync(id);
                if (item == null)
                    return ResponseDto<bool>.FailureResponse("Menu item not found");

                item.IsAvailable = !item.IsAvailable;
                item.UpdatedAt = DateTime.UtcNow;

                await _menuItemRepository.UpdateAsync(item);
                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.SuccessResponse(true, $"Availability toggled successfully. Item is now {(item.IsAvailable ? "available" : "unavailable")}");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailureResponse($"Error: {ex.Message}");
            }
        }

        #endregion
    }
}