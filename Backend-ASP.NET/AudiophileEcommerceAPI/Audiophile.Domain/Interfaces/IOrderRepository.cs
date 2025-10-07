using Audiophile.Domain.Models;

namespace Audiophile.Domain.Interfaces
{
    public interface IOrderRepository
    {

        // Transaction management
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<IEnumerable<Order>> GetAllOrders();
        Task<Order?> GetOrderById(int id);
        Task<Order> CreateOrder(Order order);
        Task<CustomerInfo?> GetCustomerByEmailAsync(string email);
        Task<bool> UpdateOrder(Order order, CustomerInfo customer);
        Task RemoveOrderItemsAsync(IEnumerable<OrderItem> items);
        Task<bool> DeleteOrder(int id);

    }
}
