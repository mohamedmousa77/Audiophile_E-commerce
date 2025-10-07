using Audiophile.Application.DTOs.Auth;
using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using Microsoft.Extensions.Logging;

//using Microsoft.Extensions.Configuration;

namespace Audiophile.Application.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, ITokenService tokenService, IPasswordHasher passwordHasher, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _logger = logger;
            
        }
        public async Task<AuthResultDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            if (await _authRepository.UserExistsByEmailAsync(registerDTO.Email))
            {
                return new AuthResultDTO
                {
                    Success = false,
                    Message = "Un utente con questa email è già registrato."
                };
            }

            try
            {
                string passwordHash = _passwordHasher.HashPassword(registerDTO.Password);

                var user = new User
                {
                    Email = registerDTO.Email,
                    PasswordHash = passwordHash,
                    Role = "Customer",
                    Phone = registerDTO.Phone,
                    FullName = registerDTO.FullName,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _authRepository.RegisterAsync(user);

                string token = _tokenService.GenerateToken(createdUser);

                return new AuthResultDTO
                {
                    Success = true,
                    Token = token,
                    Message = "Registrazione completata con successo",
                    Expiration = _tokenService.GetTokenExpiration(token),
                    User = new UserDTO
                    {
                        Id = createdUser.Id,
                        FullName = createdUser.FullName,
                        Email = createdUser.Email,
                        Phone = createdUser.Phone,
                        Role = createdUser.Role,
                    }
                };
            } 
            catch (Exception)
            {
                // Log l'errore (usa ILogger)
                return new AuthResultDTO
                {
                    Success = false,
                    Message = "Si è verificato un errore durante la registrazione."
                };
            }
        }

        public async Task<AuthResultDTO> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _authRepository.GetUserByEmailAsync(loginDTO.Email);

            if (user == null)
            {
                _logger.LogWarning("Tentativo di login fallito per email: {Email}", loginDTO.Email);  // ✅
                return new AuthResultDTO
                {
                    Success = false,
                    Message = "Email o password non corretti"
                };
            }

            bool isPasswordValid = _passwordHasher.VerifyPassword(loginDTO.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                _logger.LogWarning("Password errata per utente: {UserId}", user.Id);
                return new AuthResultDTO
                {
                    Success = false,
                    Message = "Email o password non corretti"
                };
            }

            _logger.LogInformation("Login riuscito per utente: {UserId}", user.Id);

            string token = _tokenService.GenerateToken(user);

            return new AuthResultDTO
            {
                Success = true,
                Token = token,
                Message = "Login effettuato con successo",
                Expiration = _tokenService.GetTokenExpiration(token),
                User = new UserDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                }
            };

        }

        public async Task<UserDTO?> GetUserByIdAsync(int userId)
        {
            var user = await _authRepository.GetByIdAsync(userId);

            if (user == null)
                return null;

            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone
            };
        }
    }
}
