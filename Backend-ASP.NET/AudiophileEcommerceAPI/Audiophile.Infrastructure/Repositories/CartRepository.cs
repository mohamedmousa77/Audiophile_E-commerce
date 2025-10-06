using Audiophile.Infrastructure.Data;
using Audiophile.Domain.Models;
using Audiophile.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Audiophile.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        public readonly AppDbContext _appDbContext;
        public CartRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        

        public async Task<bool> ClearCart(int customerId)
        {
            var cart = await GetCartByOnlyCustomerId(customerId);
            if (cart == null) return false;            

            _appDbContext.CartItems.RemoveRange(cart.Items!);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Cart?> GetCartByCustomerId(int customerId)
        {
            return await _appDbContext.Carts
                .Include(c => c.Items)!
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerInfoId == customerId);
        }

        public async Task<Cart?> GetCartByOnlyCustomerId(int customerId)
        {
            return await _appDbContext.Carts
                .FirstOrDefaultAsync(c => c.CustomerInfoId == customerId);
        }

        public async Task<Cart> SaveCart(Cart cart)
        {
            if (cart.Id == 0)
            {
                _appDbContext.Carts.Add(cart);
            }
            else
            {
                _appDbContext.Carts.Update(cart);
            }
            await _appDbContext.SaveChangesAsync();
            return cart;
        }
    }
}
