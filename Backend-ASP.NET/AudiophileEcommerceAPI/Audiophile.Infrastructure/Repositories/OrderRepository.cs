using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using Audiophile.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace Audiophile.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public readonly AppDbContext _appDbContext;
        private IDbContextTransaction? _transaction;
        public OrderRepository(AppDbContext appDbContext) { 
            _appDbContext = appDbContext; 
        }

        // ===== CRUD OPERATIONS =====
        public async Task<Order> CreateOrder(Order order)
        {
            _appDbContext.CustomerInfos.Add(order.CustomerInfo);
            _appDbContext.Orders.Add(order);
            await _appDbContext.SaveChangesAsync();

            await _appDbContext.Entry(order)
                .Collection(o => o.Items)
                .Query()
                .Include(i => i.Product)
                .LoadAsync();

            return order;
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _appDbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.CustomerInfo)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _appDbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.CustomerInfo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _appDbContext.Orders
                  .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.CustomerInfo)
                .Where(o => o.CustomerInfo.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

       
        public async Task<CustomerInfo?> GetCustomerByEmailAsync(string email)
        {
            return await _appDbContext.CustomerInfos
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> UpdateOrder(Order order, CustomerInfo customer)
        {
            _appDbContext.CustomerInfos.Update(customer);
            _appDbContext.Orders.Update(order);
            await _appDbContext.SaveChangesAsync();
            return true;

        }
        public async Task<bool> DeleteOrder(int id)
        {
            var order = await _appDbContext.Orders
               .Include(o => o.Items)
               .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return false;

            _appDbContext.OrderItems.RemoveRange(order.Items);
            _appDbContext.Orders.Remove(order);

            var customer = await _appDbContext.CustomerInfos.FindAsync(order.CustomerInfoId);
            if (customer != null)
            {
                _appDbContext.CustomerInfos.Remove(customer);
            }

            await _appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task RemoveOrderItemsAsync(IEnumerable<OrderItem> items)
        {
            _appDbContext.OrderItems.RemoveRange(items);
            await _appDbContext.SaveChangesAsync();
        }

        // ===== TRANSACTION MANAGEMENT =====
        public async Task BeginTransactionAsync()
        {
            _transaction = await _appDbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _appDbContext.SaveChangesAsync();

                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;    
            }
        }
    }
}
