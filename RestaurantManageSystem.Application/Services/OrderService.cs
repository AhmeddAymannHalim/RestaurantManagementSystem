using AutoMapper;
using RestaurantManageSystem.Application.DTOs.Common;
using RestaurantManageSystem.Application.DTOs.Order;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Domain.Enums;

namespace RestaurantManageSystem.Application.Services
{
    public class OrderService(IUnitOfWork unitOfWork, IMapper mapper) : IOrderService
    {
        public async Task<ResponseDto<OrderDto>> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await unitOfWork.Orders.GetByIdAsync(id);
                if (order == null)
                    return ResponseDto<OrderDto>.FailureResponse("Order not found");

                var orderDto = mapper.Map<OrderDto>(order);
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
                var orders = await unitOfWork.Orders.GetAllAsync();

                if (status.HasValue)
                {
                    orders = orders.Where(o => o.Status == status.Value);
                }

                if (date.HasValue)
                {
                    orders = orders.Where(o => o.OrderDate.Date == date.Value.Date);
                }

                var totalRecords = orders.Count();
                var items = orders
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var orderDtos = mapper.Map<List<OrderDto>>(items);
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
                var orders = await unitOfWork.Orders.FindAsync(o =>
                    o.Status == OrderStatus.Pending ||
                    o.Status == OrderStatus.Preparing ||
                    o.Status == OrderStatus.Ready);

                var orderDtos = mapper.Map<List<OrderDto>>(orders);
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
                var table = await unitOfWork.Tables.GetByIdAsync(dto.TableId);
                if (table == null)
                    return ResponseDto<OrderDto>.FailureResponse("Table not found");

                if (table.Status != TableStatus.Available)
                    return ResponseDto<OrderDto>.FailureResponse("Table is not available");

                var user = await unitOfWork.Users.GetByIdAsync(dto.UserId);
                if (user == null)
                    return ResponseDto<OrderDto>.FailureResponse("User not found");

                var order = new Order
                {
                    TableId = dto.TableId,
                    UserId = dto.UserId,
                    OrderNumber = GenerateOrderNumber(),
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var itemDto in dto.Items)
                {
                    var menuItem = await unitOfWork.MenuItems.GetByIdAsync(itemDto.MenuItemId);
                    if (menuItem == null)
                        return ResponseDto<OrderDto>.FailureResponse($"Menu item {itemDto.MenuItemId} not found");

                    if (!menuItem.IsAvailable)
                        return ResponseDto<OrderDto>.FailureResponse($"Menu item {menuItem.Name} is not available");

                    var orderItem = new OrderItem
                    {
                        MenuItemId = menuItem.Id,
                        Quantity = itemDto.Quantity,
                        UnitPrice = menuItem.Price,
                        Subtotal = menuItem.Price * itemDto.Quantity,
                        SpecialRequest = itemDto.SpecialRequest,
                        CreatedAt = DateTime.UtcNow
                    };

                    order.OrderItems.Add(orderItem);
                }

                order.Subtotal = order.OrderItems.Sum(oi => oi.Subtotal);
                order.Tax = order.Subtotal * 0.14m;
                order.TotalAmount = order.Subtotal + order.Tax;

                table.Status = TableStatus.Occupied;
                table.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.Orders.AddAsync(order);
                await unitOfWork.Tables.UpdateAsync(table);
                await unitOfWork.SaveChangesAsync();

                var orderDto = mapper.Map<OrderDto>(order);
                return ResponseDto<OrderDto>.SuccessResponse(orderDto, "Order created successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<OrderDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<OrderDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                    return ResponseDto<OrderDto>.FailureResponse("Order not found");

                var newStatus = Enum.Parse<OrderStatus>(dto.Status);

                if (!IsValidStatusTransition(order.Status, newStatus))
                    return ResponseDto<OrderDto>.FailureResponse($"Cannot change status from {order.Status} to {newStatus}");

                order.Status = newStatus;
                order.UpdatedAt = DateTime.UtcNow;

                if (newStatus == OrderStatus.Served)
                {
                    var table = await unitOfWork.Tables.GetByIdAsync(order.TableId);
                    if (table != null)
                    {
                        table.Status = TableStatus.Available;
                        table.UpdatedAt = DateTime.UtcNow;
                        await unitOfWork.Tables.UpdateAsync(table);
                    }
                }

                await unitOfWork.Orders.UpdateAsync(order);
                await unitOfWork.SaveChangesAsync();

                var orderDto = mapper.Map<OrderDto>(order);
                return ResponseDto<OrderDto>.SuccessResponse(orderDto, "Order status updated successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<OrderDto>.FailureResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto<bool>> CancelOrderAsync(int orderId)
        {
            try
            {
                var order = await unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                    return ResponseDto<bool>.FailureResponse("Order not found");

                if (order.Status == OrderStatus.Served)
                    return ResponseDto<bool>.FailureResponse("Cannot cancel a served order");

                order.Status = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;

                var table = await unitOfWork.Tables.GetByIdAsync(order.TableId);
                if (table != null)
                {
                    table.Status = TableStatus.Available;
                    table.UpdatedAt = DateTime.UtcNow;
                    await unitOfWork.Tables.UpdateAsync(table);
                }

                await unitOfWork.Orders.UpdateAsync(order);
                await unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.SuccessResponse(true, "Order cancelled successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailureResponse($"Error: {ex.Message}");
            }
        }

        private static string GenerateOrderNumber()
        {
            var date = DateTime.UtcNow;
            var random = new Random().Next(1000, 9999);
            return $"ORD-{date:yyyyMMdd}-{random}";
        }

        private static bool IsValidStatusTransition(OrderStatus current, OrderStatus next)
        {
            return (current, next) switch
            {
                (OrderStatus.Pending, OrderStatus.Preparing) => true,
                (OrderStatus.Pending, OrderStatus.Cancelled) => true,
                (OrderStatus.Preparing, OrderStatus.Ready) => true,
                (OrderStatus.Preparing, OrderStatus.Cancelled) => true,
                (OrderStatus.Ready, OrderStatus.Served) => true,
                (OrderStatus.Ready, OrderStatus.Cancelled) => true,
                _ => false
            };
        }
    }
}