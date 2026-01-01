using Microsoft.EntityFrameworkCore.Storage;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Infrastructure.Data;
using RestaurantManageSystem.Infrastructure.Repositories.OrderRep;
using RestaurantManageSystem.Infrastructure.Repositories.MenuRep;
using RestaurantManageSystem.Infrastructure.Repositories.CategoryRep;
using RestaurantManageSystem.Infrastructure.Repositories.TableRep;

namespace RestaurantManageSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        private IRepository<User>? _users;
        private IRepository<Category>? _categories;
        private IRepository<MenuItem>? _menuItems;
        private IRepository<Table>? _tables;
        private IRepository<Order>? _orders;
        private IRepository<OrderItem>? _orderItems;
        private IRepository<Setting>? _settings;
        private IRepository<PasswordResetToken>? _passwordResetTokens;
        private IRepository<AuditLog>? _auditLogs;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IRepository<User> Users =>
            _users ??= new Repository<User>(_context);

        public IRepository<Category> Categories =>
            _categories ??= new CategoryRepository(_context);

        public IRepository<MenuItem> MenuItems =>
            _menuItems ??= new MenuItemRepository(_context);

        public IRepository<Table> Tables =>
            _tables ??= new TableRepository(_context);

        public IRepository<Order> Orders =>
            _orders ??= new OrderRepository(_context);

        public IRepository<OrderItem> OrderItems =>
            _orderItems ??= new Repository<OrderItem>(_context);

        public IRepository<Setting> Settings =>
          _settings ??= new Repository<Setting>(_context);

        public IRepository<PasswordResetToken> PasswordResetTokens =>
            _passwordResetTokens ??= new Repository<PasswordResetToken>(_context);

        public IRepository<AuditLog> AuditLogs =>
            _auditLogs ??= new Repository<AuditLog>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}   