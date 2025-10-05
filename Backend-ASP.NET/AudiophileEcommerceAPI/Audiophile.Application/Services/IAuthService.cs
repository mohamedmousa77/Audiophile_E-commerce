using Audiophile.Application.DTOs;

using Audiophile.Domain.Models;

namespace Audiophile.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResult> AuthenticateCustomer(string email, string password);
        Task<AuthResult> RegisterCustomer(RegisterDto dto);

    }
}
