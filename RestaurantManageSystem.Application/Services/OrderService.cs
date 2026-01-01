using AutoMapper;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.Order;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Domain.Enums;

namespace RestaurantManageSystem.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IRepository<Order> orderRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<OrderDto>> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return ResponseDto<OrderDto>.FailureResponse("Order not found");

                var orderDto = _mapper.Map<OrderDto>(order);
                return ResponseDto<OrderDto>.SuccessResponse(orderDto);
            }
            catch (Exception ex)
            {
                return ResponseDto<OrderDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<PaginatedResultDto<OrderDto>>> GetOrdersAsync(OrderStatus? status, DateTime? date, int page, int pageSize)
        {
            try
            {
                var allOrders = (await _orderRepository.GetAllAsync()).AsQueryable();

                if (status.HasValue)
                    allOrders = allOrders.Where(o => o.Status == status.Value);

                if (date.HasValue)
                    allOrders = allOrders.Where(o => o.OrderDate.Date == date.Value.Date);

                var totalRecords = allOrders.Count();
                var items = allOrders
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var orderDtos = _mapper.Map<List<OrderDto>>(items);
                var result = PaginatedResultDto<OrderDto>.Create(orderDtos, page, pageSize, totalRecords);

                return ResponseDto<PaginatedResultDto<OrderDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ResponseDto<PaginatedResultDto<OrderDto>>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<OrderDto>>> GetActiveOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.FindAsync(o =>
                    o.Status == OrderStatus.Pending ||
                    o.Status == OrderStatus.Preparing ||
                    o.Status == OrderStatus.Ready);

                var orderDtos = _mapper.Map<List<OrderDto>>(orders);
                return ResponseDto<List<OrderDto>>.SuccessResponse(orderDtos);
            }
            catch (Exception ex)
            {
                return ResponseDto<List<OrderDto>>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<OrderDto>> CreateOrderAsync(CreateOrderDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var order = new Order
                {
                    TableId = dto.TableId,
                    UserId = dto.UserId,
                    Notes = dto.Notes,
                    OrderItems = dto.Items.Select(i => new OrderItem
                    {
                        MenuItemId = i.MenuItemId,
                        Quantity = i.Quantity,
                        SpecialRequest = i.SpecialRequest
                    }).ToList()
                };

                await _orderRepository.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var createdOrder = await _orderRepository.GetByIdAsync(order.Id);
                var orderDto = _mapper.Map<OrderDto>(createdOrder);

                return ResponseDto<OrderDto>.SuccessResponse(orderDto, "Order created successfully");
            }
            catch (Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                catch { }
                return ResponseDto<OrderDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<OrderDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return ResponseDto<OrderDto>.FailureResponse("Order not found");

                var newStatus = Enum.Parse<OrderStatus>(dto.Status);
                order.Status = newStatus;
                order.UpdatedAt = DateTime.UtcNow;

                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                var updatedOrder = await _orderRepository.GetByIdAsync(orderId);
                var orderDto = _mapper.Map<OrderDto>(updatedOrder);

                return ResponseDto<OrderDto>.SuccessResponse(orderDto, "Order status updated successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<OrderDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> CancelOrderAsync(int orderId, string? reason = null)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ResponseDto<bool>.FailureResponse("Order not found");
                }

                if (order.Status == OrderStatus.Served || order.Status == OrderStatus.Cancelled)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ResponseDto<bool>.FailureResponse($"Cannot cancel order with status: {order.Status}");
                }

                order.Status = OrderStatus.Cancelled;
                order.CancelledAt = DateTime.UtcNow;
                order.CancellationReason = reason;
                order.UpdatedAt = DateTime.UtcNow;

                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return ResponseDto<bool>.SuccessResponse(true, "Order cancelled successfully");
            }
            catch (Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                catch { }
                return ResponseDto<bool>.FailureResponse($"Error: {ex.Message}");
            }
        }
    }
}