using Audiophile.Application.DTOs.Auth;
using Audiophile.Domain.Models;

namespace Audiophile.Application.Services
{
    public interface IAuthService
    {

        Task<AuthResultDTO> RegisterAsync(RegisterDTO registerDTO);
        Task<AuthResultDTO> LoginAsync(LoginDTO loginDTO);
        Task<UserDTO?> GetUserByIdAsync(int userId);

    }

    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public interface ITokenService
    {
        string GenerateToken(User user);
        DateTime GetTokenExpiration(string token);
    }

}
