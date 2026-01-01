using Microsoft.EntityFrameworkCore;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Infrastructure.Data;
using System.Linq.Expressions;

namespace RestaurantManageSystem.Infrastructure.Repositories.MenuRep
{
    public class MenuItemRepository : IRepository<MenuItem>
    {
        private readonly AppDbContext _context;

        public MenuItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MenuItem?> GetByIdAsync(int id)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> FindAsync(Expression<Func<MenuItem, bool>> predicate)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<MenuItem> AddAsync(MenuItem entity)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == entity.CategoryId);
            if (!categoryExists)
                throw new Exception("Category not found");

            if (entity.Price <= 0)
                throw new Exception("Price must be greater than zero");

            var existingItem = await _context.MenuItems
                .AnyAsync(m => m.Name == entity.Name && m.CategoryId == entity.CategoryId);
            if (existingItem)
                throw new Exception($"Menu item '{entity.Name}' already exists in this category");

            entity.CreatedAt = DateTime.UtcNow;

            await _context.MenuItems.AddAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(MenuItem entity)
        {
            var existing = await _context.MenuItems.FindAsync(entity.Id);
            if (existing == null)
                throw new Exception("Menu item not found");

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == entity.CategoryId);
            if (!categoryExists)
                throw new Exception("Category not found");

            if (entity.Price <= 0)
                throw new Exception("Price must be greater than zero");

            var duplicateItem = await _context.MenuItems
                .AnyAsync(m => m.Name == entity.Name && m.CategoryId == entity.CategoryId && m.Id != entity.Id);
            if (duplicateItem)
                throw new Exception($"Menu item '{entity.Name}' already exists in this category");

            entity.UpdatedAt = DateTime.UtcNow;
            _context.MenuItems.Update(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null)
                throw new Exception("Menu item not found");

            var usedInOrders = await _context.OrderItems.AnyAsync(oi => oi.MenuItemId == id);
            if (usedInOrders)
                throw new Exception("Cannot delete menu item that has been ordered");

            _context.MenuItems.Remove(item);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MenuItems.AnyAsync(m => m.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.MenuItems.CountAsync();
        }
    }
}