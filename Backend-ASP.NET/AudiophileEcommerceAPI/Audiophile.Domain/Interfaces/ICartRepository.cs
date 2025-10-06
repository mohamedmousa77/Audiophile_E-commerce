using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Audiophile.Domain.Models;

namespace Audiophile.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByCustomerId(int customerId);
        Task<Cart> SaveCart(Cart cart);
        Task<bool> ClearCart(int customerId);
    }
}
