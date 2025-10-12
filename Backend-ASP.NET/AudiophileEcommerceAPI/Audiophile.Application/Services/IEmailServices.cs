using Audiophile.Domain.Models;

namespace Audiophile.Application.Services
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(User user, string token);
        Task SendPasswordResetAsync(User user, string token);
        Task SendOrderConfirmationAsync(Order order);
        Task SendOrderCancellationAsync(Order order);
        Task SendWelcomeEmailAsync(User user);
    }
}
