using AudiophileEcommerceAPI.DTOs;
using AudiophileEcommerceAPI.Models;

namespace AudiophileEcommerceAPI.Services
{
    public interface IAuthService
    {
        Task<CustomerInfo?> AuthenticateCustomer(string email, string password);
        Task<AuthResult> RegisterCustomer(RegisterDto dto);
        string GenerateJwtToken(CustomerInfo user);

    }
}
