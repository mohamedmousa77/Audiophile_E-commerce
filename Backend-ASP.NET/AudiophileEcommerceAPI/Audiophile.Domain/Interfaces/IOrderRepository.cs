using AudiophileEcommerceAPI.DTOs;
using AudiophileEcommerceAPI.Models;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace AudiophileEcommerceAPI.Services
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderDTO>> GetAllOrders();
        Task<OrderDTO?> GetOrderById(int id);
        Task<Order> CreateOrder(OrderDTO orderDTO);
        Task<bool> UpdateOrder(OrderDTO orderDTO);
        Task<bool> DeleteOrder(int id);

    }
}
