using RestaurantManageSystem.Domain.Entities;

namespace RestaurantManageSystem.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Category> Categories { get; }
        IRepository<MenuItem> MenuItems { get; }
        IRepository<Table> Tables { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderItem> OrderItems { get; }
        IRepository<Setting> Settings { get; }  

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}