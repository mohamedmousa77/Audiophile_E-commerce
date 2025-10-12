using static Audiophile.Application.DTOs.OrderDTO;

namespace Audiophile.Application.Services
{
    public interface IOrderService
    {
        Task<OrderReadDTO> ProcessOrder(OrderCreateDTO dto, int userId);
        Task<OrderReadDTO?> GetOrderById(int orderId, int userId, bool isAdmin);
        Task<PagedResult<OrderReadDTO>> GetUserOrdersAsync(int userId, int pageNumber, int pageSize);
        Task<PagedResult<OrderReadDTO>> GetAllOrdersAsync(int pageNumber, int pageSize, string? status);
        Task<OrderReadDTO?> UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> UpdateOrder(UpdateOrderDTO dto, int userId);
        Task<bool> CancelOrderAsync(int orderId, int userId, string reason);
    }
}
