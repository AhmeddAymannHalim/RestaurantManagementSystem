using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManageSystem.Application.DTOs.Category;
using RestaurantManageSystem.Application.DTOs.MenuItem;
using RestaurantManageSystem.Application.Interfaces;

namespace RestaurantManageSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        #region Categories

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _menuService.GetAllCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _menuService.GetCategoryByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("categories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var result = await _menuService.CreateCategoryAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Data.Id }, result);
        }

        [HttpPut("categories/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            var result = await _menuService.UpdateCategoryAsync(id, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("categories/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _menuService.DeleteCategoryAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion

        #region Menu Items

        [HttpGet("items")]
        public async Task<IActionResult> GetMenuItems([FromQuery] int? categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _menuService.GetMenuItemsAsync(categoryId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("items/{id}")]
        public async Task<IActionResult> GetMenuItemById(int id)
        {
            var result = await _menuService.GetMenuItemByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("items")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMenuItem([FromBody] CreateMenuItemDto dto)
        {
            var result = await _menuService.CreateMenuItemAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetMenuItemById), new { id = result.Data.Id }, result);
        }

        [HttpPut("items/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] UpdateMenuItemDto dto)
        {
            var result = await _menuService.UpdateMenuItemAsync(id, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("items/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var result = await _menuService.DeleteMenuItemAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("items/{id}/toggle-availability")]
        [Authorize(Roles = "Admin,Kitchen")]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var result = await _menuService.ToggleAvailabilityAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion
    }
}