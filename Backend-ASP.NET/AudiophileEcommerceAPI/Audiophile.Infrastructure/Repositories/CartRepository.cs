using AudiophileEcommerceAPI.Data;
using AudiophileEcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AudiophileEcommerceAPI.Services
{
    public class CartRepository : ICartRepository
    {
        public readonly AppDbContext _appDbContext;
        public CartRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> AddToCart(int customerId, int productId, int quantity)
        {
            if (quantity <= 0) return false;

            var cart = await GetCartByCustomerId( customerId);
            if (cart == null) return false;
            
            var existingItem = cart.Items!.FirstOrDefault(item => item.ProductId == productId);                     
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    CartId = cart.Id
                };
                cart.Items!.Add(newItem);
            }

            await _appDbContext.SaveChangesAsync(); 
            return true;
        }

        public async Task<bool> ClearCart(int customerId)
        {
            Cart cart = await GetCartByCustomerId(customerId);
            if (cart == null) return false;            
            _appDbContext.CartItems.RemoveRange(cart.Items);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Cart> GetCartByCustomerId(int customerId)
        {
            var cart =await _appDbContext.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerInfoId == customerId);

            if (cart == null)
            {
                cart = new Cart { CustomerInfoId = customerId, Items = new List<CartItem>() };

                _appDbContext.Carts.Add(cart);
                await _appDbContext.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<bool> RemoveFromCart(int customerId, int productId)
        {
            Cart cart = await GetCartByCustomerId(customerId);
            if (cart == null) return false;
            var itemToRemove = cart.Items.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove == null) return false;

            _appDbContext.CartItems.Remove(itemToRemove);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCartItem(int customerId, int productId, int quantity)
        {
            var cart = await GetCartByCustomerId(customerId);
            if (cart == null) return false;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return false;

            if (quantity <= 0)
            {
                _appDbContext.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            await _appDbContext.SaveChangesAsync();
            return true;


        }
    }
}
