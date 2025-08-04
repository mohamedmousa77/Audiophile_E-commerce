using AudiophileEcommerceAPI.Data;
using AudiophileEcommerceAPI.DTOs;
using AudiophileEcommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AudiophileEcommerceAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResult> AuthenticateCustomer(string email, string password)
        {
            var user = await _context.Customers.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password)) // replace with hash check
            {
                return new AuthResult { Success = false, Message = "Invalid credentials" };
            }

            var token = GenerateJwtToken(user);
            return new AuthResult { Success = true, Token = token };

        }

        public async Task<AuthResult> RegisterCustomer([FromBody] RegisterDto dto)
        {
            if (_context.Customers.Any(x => x.Email == dto.Email))
                return new AuthResult { Success = false, Message = "Email already in use" };

            var customer = new CustomerInfo
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                ZipCode = dto.ZipCode,
            };
            customer.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(customer);
            return new AuthResult { Success = true, Token = token };
        }
        string GenerateJwtToken(CustomerInfo user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
