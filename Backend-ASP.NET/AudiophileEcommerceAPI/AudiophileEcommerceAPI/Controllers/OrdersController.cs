using Asp.Versioning;
using Audiophile.Application.Services;
using Audiophile.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using static Audiophile.Application.DTOs.OrderDTO;

namespace AudiophileEcommerceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [EnableRateLimiting("api")]
    public class OrdersController: ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        public OrdersController(OrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }
        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderReadDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO newOrder)
        {
            var userId = GetUserId();
            if (userId == 0)
                return Unauthorized();
            try
            {
                var order = await _orderService.ProcessOrder(newOrder, userId);
                return CreatedAtAction(
                    nameof(GetOrderById),
                    new { id = order.Id },
                    order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }      

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<OrderReadDTO>> GetOrderById(int id)
        {
            var userId = GetUserId();
            if (userId == 0)
                return Unauthorized();
            try
            {

                var order = await _orderService.GetOrderById(id, userId, IsAdmin());
                if (order == null)
                {
                    return NotFound(new { message = $"Ordine {id} non trovato" });
                }
                return Ok(order);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-orders")]
        [ProducesResponseType(typeof(PagedResult<OrderReadDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();
            if (userId == 0)
            {
                return Unauthorized();
            }

            var result = await _orderService.GetUserOrdersAsync(userId, pageNumber, pageSize);

            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Total-Pages", result.TotalPages.ToString());

            return Ok(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("all")]
        [ProducesResponseType(typeof(PagedResult<OrderReadDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllOrders(
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 20,
           [FromQuery] string? status = null)
        {
            var result = await _orderService.GetAllOrdersAsync(pageNumber, pageSize, status); // GetAllOrdersAsync to be implemented

            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Total-Pages", result.TotalPages.ToString());

            return Ok(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(OrderReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDTO dto)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, dto.Status); // UpdateOrderStatusAsync to be implemented

                if (order == null)
                {
                    return NotFound(new { message = "Ordine non trovato" });
                }

                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderDTO dto)
        {
            var userId = GetUserId();
            if (userId == 0)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _orderService.CancelOrderAsync(id, userId, dto.Reason);

                if (!result)
                {
                    return NotFound(new { message = "Ordine non trovato" });
                }

                return Ok(new { message = "Ordine cancellato con successo" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
