using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.Order;
using RestaurantManageSystem.Domain.Enums;

namespace RestaurantManageSystem.Application.Interfaces
{
    public interface IOrderService
    {
        Task<ResponseDto<OrderDto>> GetOrderByIdAsync(int id);
        Task<ResponseDto<PaginatedResultDto<OrderDto>>> GetOrdersAsync(OrderStatus? status, DateTime? date, int page, int pageSize);
        Task<ResponseDto<List<OrderDto>>> GetActiveOrdersAsync();
        Task<ResponseDto<OrderDto>> CreateOrderAsync(CreateOrderDto dto);
        Task<ResponseDto<OrderDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
        Task<ResponseDto<bool>> CancelOrderAsync(int orderId);
    }
}