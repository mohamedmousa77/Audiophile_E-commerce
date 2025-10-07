using Audiophile.Application.Services;
using Audiophile.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Audiophile.Application.Services.AuthServices
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly byte[] _key;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Carica la chiave segreta e la codifica
            _key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]
                                           ?? throw new ArgumentNullException("JwtSettings:Secret not found."));
        }
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");
            var expirationTime = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            };

            // 3. Crea e scrive il Token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public DateTime GetTokenExpiration(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            return jwtToken?.ValidTo ?? DateTime.UtcNow;
        }
    }
}
