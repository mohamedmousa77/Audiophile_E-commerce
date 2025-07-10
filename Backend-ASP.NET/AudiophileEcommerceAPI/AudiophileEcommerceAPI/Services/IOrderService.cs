using AudiophileEcommerceAPI.DTOs;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace AudiophileEcommerceAPI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllOrders();
        Task<OrderDTO?> GetOrderById(int id);
        Task<OrderDTO> CreateOrder(OrderDTO orderDTO);
        Task<bool> UpdateOrder(OrderDTO orderDTO);
        Task<bool> DeleteOrder(int id);

    }
}
