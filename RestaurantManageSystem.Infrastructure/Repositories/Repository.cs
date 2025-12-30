using Microsoft.EntityFrameworkCore;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain._Common;
using RestaurantManageSystem.Infrastructure.Data;
using System.Linq.Expressions;

namespace RestaurantManageSystem.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            // Load all navigation properties
            var query = _dbSet.AsQueryable();

            // Get all navigation properties
            var navigations = _context.Model.FindEntityType(typeof(T))?
                .GetNavigations()
                .Select(n => n.Name);

            if (navigations != null)
            {
                foreach (var property in navigations)
                {
                    query = query.Include(property);
                }
            }

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // Load all navigation properties
            var query = _dbSet.AsQueryable();

            var navigations = _context.Model.FindEntityType(typeof(T))?
                .GetNavigations()
                .Select(n => n.Name);

            if (navigations != null)
            {
                foreach (var property in navigations)
                {
                    query = query.Include(property);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            // Load all navigation properties
            var query = _dbSet.AsQueryable();

            var navigations = _context.Model.FindEntityType(typeof(T))?
                .GetNavigations()
                .Select(n => n.Name);

            if (navigations != null)
            {
                foreach (var property in navigations)
                {
                    query = query.Include(property);
                }
            }

            return await query.Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}