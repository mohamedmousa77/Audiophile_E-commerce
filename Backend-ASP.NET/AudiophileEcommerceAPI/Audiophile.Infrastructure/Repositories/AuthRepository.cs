using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using Audiophile.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Audiophile.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _appDbContext;

        public AuthRepository(AppDbContext context)
        {
            _appDbContext = context;
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _appDbContext.Users.
                FirstOrDefaultAsync(u =>  u.Email.ToLower() == email.ToLower());
        }

        public async Task<User> RegisterAsync(User user)
        {
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _appDbContext.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }


        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _appDbContext.Users.
                AnyAsync(u => u.Email.ToLower() == email.ToLower());

        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            _appDbContext.RefreshTokens.Add(refreshToken);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _appDbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _appDbContext.RefreshTokens.Update(refreshToken);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task RevokeAllRefreshTokensAsync(int userId)
        {
            var tokens = await _appDbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await _appDbContext.SaveChangesAsync();
        }
    }
}
