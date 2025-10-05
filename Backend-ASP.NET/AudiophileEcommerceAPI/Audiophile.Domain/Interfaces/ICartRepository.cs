using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Audiophile.Domain.Models;

namespace Audiophile.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByCustomerId(int customerId);
        Task<bool> AddToCart(int customerId, int productId, int quantity);
        Task<bool> ClearCart(int customerId);
        Task<bool> RemoveFromCart(int customerId, int productId);
        Task<bool> UpdateCartItem(int customerId, int productId, int quantity);
    }
}
