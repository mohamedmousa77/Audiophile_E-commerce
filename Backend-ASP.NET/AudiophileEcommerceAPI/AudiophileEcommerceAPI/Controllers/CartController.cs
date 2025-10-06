using Audiophile.Application.Services;
using static Audiophile.Application.DTOs.CartDTOs;
using Microsoft.AspNetCore.Mvc;

namespace AudiophileEcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPut("{customerId}/items")]
        public async Task<ActionResult<CartReadDTO>> AddOrUpdateItem(int customerId, [FromBody] CartItemUpdateDTO dto)
        {
            if (dto.CustomerId != customerId)
            {
                return BadRequest("Customer ID mismatch in URL and body");
            }
            try
            {
                var updatedCart = await _cartService.AddOrUpdate(dto);
                return Ok(updatedCart);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new {message= ex.Message});
            }
        }

        [HttpDelete("{customerId}/items/{productId}")]
        public async Task<ActionResult<CartReadDTO>> RemoveItem(int customerId, int productId)
        {
            try
            {
                var updatedCart = await _cartService.RemoveItem(customerId, productId);
                return Ok(updatedCart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while removing the item.");
            }
        }

        [HttpGet("{customerId}")]
        public async Task<ActionResult<CartReadDTO>> GetCartByCustomerID(int customerId)
        {
            var cart = await _cartService.GetCart(customerId);
            if (cart == null)
            {
                return NotFound("Cart not found or empty.");
            }
            return Ok(cart);
        }

        [HttpDelete("{customerId}/clear")]
        public async Task<IActionResult> ClearCart(int customerId)
        {
            var cleared = await _cartService.ClearCart(customerId);

            return cleared ? NoContent() : NotFound("Cart not found or failed to clear.");
        }
    }
}
