using AudiophileEcommerceAPI.DTOs;
using AudiophileEcommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AudiophileEcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController: ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO orderDTO)
        {
            try
            {
                var order = await _orderService.CreateOrder(orderDTO);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDTO orderDto)
        {
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
