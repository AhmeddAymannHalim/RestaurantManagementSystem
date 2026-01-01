using Microsoft.EntityFrameworkCore;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Infrastructure.Data;
using System.Linq.Expressions;

namespace RestaurantManageSystem.Infrastructure.Repositories.CategoryRep
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.MenuItems)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.MenuItems)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> FindAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _context.Categories
                .Include(c => c.MenuItems)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Category> AddAsync(Category entity)
        {
            var existingCategory = await _context.Categories
                .AnyAsync(c => c.CategoryName == entity.CategoryName);
            if (existingCategory)
                throw new Exception($"Category '{entity.CategoryName}' already exists");

            entity.CreatedAt = DateTime.UtcNow;
            if (entity.DisplayOrder == 0)
            {
                var maxOrder = await _context.Categories.MaxAsync(c => (int?)c.DisplayOrder) ?? 0;
                entity.DisplayOrder = maxOrder + 1;
            }

            await _context.Categories.AddAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(Category entity)
        {
            var existing = await _context.Categories.FindAsync(entity.Id);
            if (existing == null)
                throw new Exception("Category not found");

            var duplicateCategory = await _context.Categories
                .AnyAsync(c => c.CategoryName == entity.CategoryName && c.Id != entity.Id);
            if (duplicateCategory)
                throw new Exception($"Category '{entity.CategoryName}' already exists");

            entity.UpdatedAt = DateTime.UtcNow;
            _context.Categories.Update(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category == null)
                throw new Exception("Category not found");

            if (category.MenuItems != null && category.MenuItems.Count > 0)
                throw new Exception("Cannot delete category with menu items. Please delete all menu items first.");

            _context.Categories.Remove(category);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Categories.CountAsync();
        }
    }
}