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

            var secret = _configuration["JwtSettings:Secret"]
                    ?? throw new InvalidOperationException("JwtSettings:Secret non configurato");
            
            if (secret.Length < 32)
            {
                throw new InvalidOperationException(
                    "JwtSettings:Secret deve essere almeno 32 caratteri (256 bit)");
            }
            // Carica la chiave segreta e la codifica
            _key = Encoding.UTF8.GetBytes(secret);
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
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true
                };
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;

                return jwtToken?.ValidTo ?? DateTime.UtcNow;
            }
            catch
            {
                return DateTime.UtcNow; // Token non valido
            }

        }
    }
}
