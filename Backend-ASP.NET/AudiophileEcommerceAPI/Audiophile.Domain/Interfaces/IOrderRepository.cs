using Audiophile.Domain.Models;

namespace Audiophile.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task<Order?> GetOrderById(int id);
        Task<Order> CreateOrder(Order orderDTO);
        Task<bool> UpdateOrder(Order orderDTO);
        Task<bool> DeleteOrder(int id);

    }
}
