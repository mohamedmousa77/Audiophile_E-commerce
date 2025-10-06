using Audiophile.Application.Services;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO newOrder)
        {
            try
            {
                var order = await _orderService.ProcessOrder(newOrder);
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
            var order = await _orderService.GetOrderById(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDTO orderDto)
        {
            if (orderDto == null || orderDto.OrderId != id) return BadRequest("Order Id mismatch");
            var updated = await _orderService.UpdateOrder(orderDto);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var deleted = await _orderService.DeleteOrder(id);
            return deleted ? NoContent() : NotFound();
        }

    }
}
