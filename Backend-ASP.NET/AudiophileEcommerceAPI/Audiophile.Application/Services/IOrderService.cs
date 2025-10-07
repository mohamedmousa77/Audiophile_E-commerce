using static Audiophile.Application.DTOs.OrderDTO;

namespace Audiophile.Application.Services
{
    public interface IOrderService
    {
        Task<OrderReadDTO> ProcessOrder(OrderCreateDTO dto, int userId);
        Task<OrderReadDTO?> GetOrderById(int orderId, int userId);
        Task<IEnumerable<OrderReadDTO>> GetAllOrders();
        Task<bool> UpdateOrder(UpdateOrderDTO dto, int userId);
        Task<bool> DeleteOrder(int orderId, int userId);
    }
}
