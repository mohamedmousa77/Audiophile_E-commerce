using Audiophile.Domain.Models;

namespace Audiophile.Domain.Interfaces
{
    public interface IOrderRepository
    {

        // Transaction management
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        // CRUD operations
        Task<Order> CreateOrder(Order order);
        Task<Order?> GetOrderById(int id);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<Order>> GetAllOrders();
        Task<bool> UpdateOrder(Order order, CustomerInfo customer);
        Task<bool> DeleteOrder(int id);

        // Customer operations
        Task<CustomerInfo?> GetCustomerByEmailAsync(string email);

        // Order items
        Task RemoveOrderItemsAsync(IEnumerable<OrderItem> items);

    }
}
