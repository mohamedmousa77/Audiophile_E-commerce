using Audiophile.Application.Services;
using Audiophile.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Audiophile.Application.DTOs.OrderDTO;

namespace AudiophileEcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController: ControllerBase
    {
        private readonly OrderService _orderService;
        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO newOrder)
        {
            var userId = GetUserId();
            if (userId == 0)
                return Unauthorized();
            try
            {
                var order = await _orderService.ProcessOrder(newOrder, userId);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadDTO>>> GetAllOrders() =>
           Ok(await _orderService.GetAllOrders());
        

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderReadDTO>> GetOrderById(int id)
        {
            var userId = GetUserId();
            if (userId == 0)
                return Unauthorized();
            try
            {

                var order = await _orderService.GetOrderById(id, userId);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDTO orderDto)
        {
            if (orderDto == null || orderDto.OrderId != id) return BadRequest("Order Id mismatch");
           var userId = GetUserId();
            if (userId == 0)
                return Unauthorized();
            try
            {
                var updated = await _orderService.UpdateOrder(orderDto, userId);
                return CreatedAtAction(nameof(GetOrderById), new { id = orderDto.OrderId}, updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var userId = GetUserId();
            if (userId == 0)
                return Unauthorized();

            try
            {
                bool deleted = await _orderService.DeleteOrder(id, userId);
                return Ok(deleted);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
