using AudiophileEcommerceAPI.Data;
using AudiophileEcommerceAPI.DTOs;

namespace AudiophileEcommerceAPI.Services
{
    public class OrderService : IOrderService
    {
        public readonly AppDbContext _appDbContext;
        public OrderService(AppDbContext appDbContext) { 
            _appDbContext = appDbContext; 
        }
        public Task<OrderDTO> CreateOrder(OrderDTO orderDTO)
        {
            
        }

        public Task<bool> DeleteOrder(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderDTO>> GetAllOrders()
        {
            throw new NotImplementedException();
        }

        public Task<OrderDTO?> GetOrderById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrder(OrderDTO orderDTO)
        {
            throw new NotImplementedException();
        }
    }
}
