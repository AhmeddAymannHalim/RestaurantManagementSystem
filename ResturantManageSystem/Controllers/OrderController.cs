using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManageSystem.Application.DTOs.Order;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Enums;
using Asp.Versioning;

namespace RestaurantManageSystem.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Get orders with optional filtering and pagination
        /// </summary>
        /// <param name="status">Order status filter</param>
        /// <param name="date">Date filter (YYYY-MM-DD)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
        /// <returns>Paginated list of orders</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrders(
            [FromQuery] OrderStatus? status, 
            [FromQuery] DateTime? date, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Max page size limit

            var result = await _orderService.GetOrdersAsync(status, date, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get all active orders (Pending, Preparing, Ready)
        /// </summary>
        /// <returns>List of active orders</returns>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveOrders()
        {
            var result = await _orderService.GetActiveOrdersAsync();
            return Ok(result);
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="dto">Order creation data</param>
        /// <returns>Created order</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Waiter")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var result = await _orderService.CreateOrderAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetOrderById), new { id = result.Data.Id }, result);
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="dto">New status</param>
        /// <returns>Updated order</returns>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Waiter,Kitchen")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Cancel an order with optional reason
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="reason">Cancellation reason</param>
        /// <returns>Cancellation result</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Waiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelOrder(int id, [FromQuery] string? reason = null)
        {
            var result = await _orderService.CancelOrderAsync(id, reason);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}