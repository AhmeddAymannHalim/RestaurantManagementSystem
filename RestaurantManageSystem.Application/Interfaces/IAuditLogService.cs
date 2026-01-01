namespace RestaurantManageSystem.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(int? userId, string action, string entity, int? entityId, object? oldValues = null, object? newValues = null);
        Task LogLoginAsync(int userId, bool success = true);
        Task LogLogoutAsync(int userId);
        Task LogPasswordChangeAsync(int userId);
        Task LogDeleteAsync(int? userId, string entity, int entityId);
        Task LogCreateAsync(int? userId, string entity, int entityId, object newValues);
        Task LogUpdateAsync(int? userId, string entity, int entityId, object oldValues, object newValues);
    }
}
