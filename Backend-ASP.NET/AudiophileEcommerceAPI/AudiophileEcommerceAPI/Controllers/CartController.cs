using AudiophileEcommerceAPI.Services;
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

        [HttpPost("{customerId}/add")]
        public async Task<ActionResult> AddToCart(int customerId, [FromQuery] int productId, [FromQuery] int quantity)
        {
            var result = await _cartService.AddToCart(customerId, productId, quantity);
            if (result)
            {
                return Ok("Item added to cart successfully.");
            }
            return BadRequest("Failed to add item to cart.");
        }

        [HttpGet("{customerId}")]
        public async Task<ActionResult> GetCart(int customerId)
        {
            var cart = await _cartService.GetCartByCustomerId(customerId);
            if (cart != null)
            {
                return Ok(cart);
            }
            return NotFound("Cart not found.");
        }

        [HttpDelete("{customerId}/remove/{productId}")]
        public async Task<ActionResult> RemoveFromCart(int customerId, int productId)
        {
            var result = await _cartService.RemoveFromCart(customerId, productId);
            if (result)
            {
                return Ok("Item removed from cart successfully.");
            }
            return BadRequest("Failed to remove item from cart.");
        }

        [HttpDelete("{customerId}/clear")]
        public async Task<ActionResult> ClearCart(int customerId)
        {
            var result = await _cartService.ClearCart(customerId);
            if (result)
            {
                return Ok("Cart cleared successfully.");
            }
            return BadRequest("Failed to clear cart.");
        }
    }
}
