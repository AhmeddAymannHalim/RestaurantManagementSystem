using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using System.Text.Json;

namespace RestaurantManageSystem.Infrastructure.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task LogAsync(int? userId, string action, string entity, int? entityId, object? oldValues = null, object? newValues = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    Entity = entity,
                    EntityId = entityId,
                    OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                    NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.AuditLogs.AddAsync(auditLog);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to log audit: {ex.Message}");
            }
        }

        public async Task LogLoginAsync(int userId, bool success = true)
        {
            var action = success ? "LOGIN_SUCCESS" : "LOGIN_FAILED";
            await LogAsync(userId, action, "User", userId);
        }

        public async Task LogLogoutAsync(int userId)
        {
            await LogAsync(userId, "LOGOUT", "User", userId);
        }

        public async Task LogPasswordChangeAsync(int userId)
        {
            await LogAsync(userId, "PASSWORD_CHANGED", "User", userId);
        }

        public async Task LogDeleteAsync(int? userId, string entity, int entityId)
        {
            await LogAsync(userId, "DELETE", entity, entityId);
        }

        public async Task LogCreateAsync(int? userId, string entity, int entityId, object newValues)
        {
            await LogAsync(userId, "CREATE", entity, entityId, null, newValues);
        }

        public async Task LogUpdateAsync(int? userId, string entity, int entityId, object oldValues, object newValues)
        {
            await LogAsync(userId, "UPDATE", entity, entityId, oldValues, newValues);
        }
    }
}
