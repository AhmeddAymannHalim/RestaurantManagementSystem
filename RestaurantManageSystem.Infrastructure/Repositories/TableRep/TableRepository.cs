using Microsoft.EntityFrameworkCore;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Domain.Enums;
using RestaurantManageSystem.Infrastructure.Data;
using System.Linq.Expressions;

namespace RestaurantManageSystem.Infrastructure.Repositories.TableRep
{
    public class TableRepository : IRepository<Table>
    {
        private readonly AppDbContext _context;

        public TableRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Table?> GetByIdAsync(int id)
        {
            return await _context.Tables
                .Include(t => t.Orders.Where(o =>
                    o.Status == OrderStatus.Pending ||
                    o.Status == OrderStatus.Preparing ||
                    o.Status == OrderStatus.Ready))
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Table>> GetAllAsync()
        {
            return await _context.Tables
                .Include(t => t.Orders.Where(o =>
                    o.Status == OrderStatus.Pending ||
                    o.Status == OrderStatus.Preparing ||
                    o.Status == OrderStatus.Ready))
                .OrderBy(t => t.TableNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Table>> FindAsync(Expression<Func<Table, bool>> predicate)
        {
            return await _context.Tables
                .Include(t => t.Orders.Where(o =>
                    o.Status == OrderStatus.Pending ||
                    o.Status == OrderStatus.Preparing ||
                    o.Status == OrderStatus.Ready))
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Table> AddAsync(Table entity)
        {
            var existingTable = await _context.Tables
                .AnyAsync(t => t.TableNumber == entity.TableNumber);
            if (existingTable)
                throw new Exception($"Table number {entity.TableNumber} already exists");

            if (entity.Capacity <= 0)
                throw new Exception("Table capacity must be greater than zero");

            entity.Status = TableStatus.Available;
            entity.IsActive = true;
            entity.CreatedAt = DateTime.UtcNow;

            await _context.Tables.AddAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(Table entity)
        {
            var existing = await _context.Tables.FindAsync(entity.Id);
            if (existing == null)
                throw new Exception("Table not found");

            var duplicateTable = await _context.Tables
                .AnyAsync(t => t.TableNumber == entity.TableNumber && t.Id != entity.Id);
            if (duplicateTable)
                throw new Exception($"Table number {entity.TableNumber} already exists");

            if (entity.Capacity <= 0)
                throw new Exception("Table capacity must be greater than zero");

            entity.UpdatedAt = DateTime.UtcNow;
            _context.Tables.Update(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var table = await GetByIdAsync(id);
            if (table == null)
                throw new Exception("Table not found");

            var hasActiveOrders = table.Orders != null && table.Orders.Any();
            if (hasActiveOrders)
                throw new Exception("Cannot delete table with active orders");

            _context.Tables.Remove(table);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Tables.AnyAsync(t => t.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Tables.CountAsync();
        }
    }
}