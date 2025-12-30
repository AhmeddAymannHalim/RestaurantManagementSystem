using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManageSystem.Application.DTOs.Order;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Domain.Enums;

namespace RestaurantManageSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] OrderStatus? status, [FromQuery] DateTime? date, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetOrdersAsync(status, date, page, pageSize);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveOrders()
        {
            var result = await _orderService.GetActiveOrdersAsync();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var result = await _orderService.CreateOrderAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetOrderById), new { id = result.Data.Id }, result);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Waiter,Kitchen")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrderAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}