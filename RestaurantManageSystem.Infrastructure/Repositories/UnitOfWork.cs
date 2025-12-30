using Microsoft.EntityFrameworkCore.Storage;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Infrastructure.Data;
using System.Runtime;

namespace RestaurantManageSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        private IRepository<User>? _users;
        private IRepository<Category>? _categories;
        private IRepository<MenuItem>? _menuItems;
        private IRepository<Domain.Entities.Table>? _tables;
        private IRepository<Order>? _orders;
        private IRepository<OrderItem>? _orderItems;
        private IRepository<Setting>? _settings;

        public IRepository<User> Users =>
            _users ??= new Repository<User>(_context);

        public IRepository<Category> Categories =>
            _categories ??= new Repository<Category>(_context);

        public IRepository<MenuItem> MenuItems =>
            _menuItems ??= new Repository<MenuItem>(_context);

        public IRepository<Domain.Entities.Table> Tables =>
            _tables ??= new Repository<Domain.Entities.Table>(_context);

        public IRepository<Order> Orders =>
            _orders ??= new Repository<Order>(_context);

        public IRepository<OrderItem> OrderItems =>
            _orderItems ??= new Repository<OrderItem>(_context);

        public IRepository<Setting> Settings =>  
          _settings ??= new Repository<Setting>(_context);

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

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