using Microsoft.EntityFrameworkCore;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Domain.Enums;
using RestaurantManageSystem.Infrastructure.Data;
using System.Linq.Expressions;

namespace RestaurantManageSystem.Infrastructure.Repositories.OrderRep
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        .ThenInclude(m => m.Category)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        .ThenInclude(m => m.Category)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> FindAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        .ThenInclude(m => m.Category)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Order> AddAsync(Order entity)
        {
            var table = await _context.Tables.FindAsync(entity.TableId);
            if (table == null)
                throw new Exception("Table not found");

            if (table.Status != TableStatus.Available)
                throw new Exception("Table is not available");

            var userExists = await _context.Users.AnyAsync(u => u.Id == entity.UserId);
            if (!userExists)
                throw new Exception("User not found");

            decimal subtotal = 0;
            foreach (var orderItem in entity.OrderItems)
            {
                var menuItem = await _context.MenuItems.FindAsync(orderItem.MenuItemId);
                if (menuItem == null)
                    throw new Exception($"Menu item {orderItem.MenuItemId} not found");

                if (!menuItem.IsAvailable)
                    throw new Exception($"Menu item {menuItem.Name} is not available");

                orderItem.UnitPrice = menuItem.Price;
                orderItem.Subtotal = menuItem.Price * orderItem.Quantity;
                orderItem.CreatedAt = DateTime.UtcNow;

                subtotal += orderItem.Subtotal;
            }

            entity.Subtotal = subtotal;
            entity.Tax = subtotal * 0.14m;
            entity.TotalAmount = entity.Subtotal + entity.Tax;
            entity.CreatedAt = DateTime.UtcNow;
            entity.OrderDate = DateTime.UtcNow;
            entity.Status = OrderStatus.Pending;
            entity.OrderNumber = GenerateOrderNumber();

            table.Status = TableStatus.Occupied;
            table.UpdatedAt = DateTime.UtcNow;

            await _context.Orders.AddAsync(entity);

            return entity;
        }

        public async Task UpdateAsync(Order entity)
        {
            var existingOrder = await _context.Orders.FindAsync(entity.Id);
            if (existingOrder == null)
                throw new Exception("Order not found");

            if (entity.Status == OrderStatus.Served && existingOrder.Status != OrderStatus.Served)
            {
                var table = await _context.Tables.FindAsync(existingOrder.TableId);
                if (table != null)
                {
                    table.Status = TableStatus.Available;
                    table.UpdatedAt = DateTime.UtcNow;
                }
            }

            if (entity.Status == OrderStatus.Cancelled && existingOrder.Status != OrderStatus.Cancelled)
            {
                var table = await _context.Tables.FindAsync(existingOrder.TableId);
                if (table != null)
                {
                    table.Status = TableStatus.Available;
                    table.UpdatedAt = DateTime.UtcNow;
                }
            }

            entity.UpdatedAt = DateTime.UtcNow;
            _context.Orders.Update(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                throw new Exception("Order not found");

            if (order.Status == OrderStatus.Served)
                throw new Exception("Cannot cancel a served order");

            var table = await _context.Tables.FindAsync(order.TableId);
            if (table != null)
            {
                table.Status = TableStatus.Available;
                table.UpdatedAt = DateTime.UtcNow;
            }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Orders.AnyAsync(o => o.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        private static string GenerateOrderNumber()
        {
            var date = DateTime.UtcNow;
            var random = new Random().Next(1000, 9999);
            return $"ORD-{date:yyyyMMdd}-{random}";
        }
    }
}