using AudiophileEcommerceAPI.Data;
using AudiophileEcommerceAPI.Models;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AudiophileEcommerceAPI.Services
{
    public interface ICartService
    {
        Task<Cart> GetCartByCustomerId(int customerId);
        Task<bool> AddToCart(int customerId, int productId, int quantity);
        Task<bool> ClearCart(int customerId);
        Task<bool> RemoveFromCart(int customerId, int productId);
        Task<bool> UpdateCartItem(int customerId, int productId, int quantity);
    }
}
