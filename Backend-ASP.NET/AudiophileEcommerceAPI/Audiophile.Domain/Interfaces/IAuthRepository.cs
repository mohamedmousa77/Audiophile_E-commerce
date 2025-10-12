using Audiophile.Domain.Models;

namespace Audiophile.Domain.Interfaces
{
   public interface IAuthRepository
   {
        Task<User> RegisterAsync(User user);
        Task<bool> UserExistsByEmailAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task UpdateUserAsync(User user);

        // Refresh Token operations
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task RevokeAllRefreshTokensAsync(int userId);

   }


}
