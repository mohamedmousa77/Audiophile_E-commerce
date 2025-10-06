using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using Audiophile.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Audiophile.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public readonly AppDbContext _appDbContext;
        public OrderRepository(AppDbContext appDbContext) { 
            _appDbContext = appDbContext; 
        }

        public async Task<Order> CreateOrder(Order order)
        {
            _appDbContext.Customers.Add(order.CustomerInfo);
            _appDbContext.Orders.Add(order);
            await _appDbContext.SaveChangesAsync();
            return order;

        }

        public async Task<bool> DeleteOrder(int id)
        {
            var order = await _appDbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return false;

            _appDbContext.OrderItems.RemoveRange(order.Items);
            _appDbContext.Orders.Remove(order);

            var customer = await _appDbContext.Customers.FindAsync(order.CustomerInfoId);
            if (customer != null)
            {
                _appDbContext.Customers.Remove(customer);
            }

            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _appDbContext.Orders
                .Include (o => o.Items)
                .Include (o => o.CustomerInfo)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _appDbContext.Orders
                .Include(o => o.Items)
                .Include (o => o.CustomerInfo)
                .FirstOrDefaultAsync(o=> o.Id == id);
        }

        public async Task<CustomerInfo?> GetCustomerByEmailAsync(string email)
        {
            return await _appDbContext.Customers
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<bool> UpdateOrder(Order order, CustomerInfo customer)
        {
            _appDbContext.Customers.Update(customer);
            _appDbContext.Orders.Update(order);
            await _appDbContext.SaveChangesAsync();
            return true;

        }

        public async Task RemoveOrderItemsAsync(IEnumerable<OrderItem> items)
        {
            _appDbContext.OrderItems.RemoveRange(items);
            await Task.CompletedTask;
        }


    }
}
