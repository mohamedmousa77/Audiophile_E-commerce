using Audiophile.Application.DTOs.Auth;
using Audiophile.Application.DTOs.AuthDTOs;
using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

//using Microsoft.Extensions.Configuration;

namespace Audiophile.Application.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;

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
                    FullName = registerDTO.FullName,
                    Email = registerDTO.Email,
                    PasswordHash = passwordHash,
                    Role = "Customer",
                    Phone = registerDTO.Phone,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = false,
                    EmailConfirmationToken = GenerateRandomToken()

                };

                var createdUser = await _authRepository.RegisterAsync(user);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendEmailConfirmationAsync(createdUser, createdUser.EmailConfirmationToken!);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Errore invio email conferma per {Email}", createdUser.Email);
                    }
                });

                string token = _tokenService.GenerateToken(createdUser);
                string refreshToken = GenerateRefreshToken();

                await SaveRefreshTokenAsync(createdUser.Id, refreshToken);


                return new AuthResultDTO
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    Message = "Registrazione completata con successo. Controlla la tua email per confermare l'account.",
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante registrazione per {Email}", registerDTO.Email);
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
                _logger.LogWarning("Tentativo di login fallito per email: {Email}", loginDTO.Email);
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
            string refreshToken = GenerateRefreshToken();

            // Salva refresh token
            await SaveRefreshTokenAsync(user.Id, refreshToken);

            return new AuthResultDTO
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
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
                Phone = user.Phone,
                Role = user.Role
            };
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            var user = await _authRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                // Non rivelare che l'email non esiste (sicurezza)
                _logger.LogWarning("Reset password richiesto per email inesistente: {Email}", email);
                return;
            }

            // Genera token
            var resetToken = GenerateRandomToken();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _authRepository.UpdateUserAsync(user);

            // Invia email (async, non blocca)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendPasswordResetAsync(user, resetToken);
                    _logger.LogInformation("Email reset password inviata a {Email}", email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore invio email reset per {Email}", email);
                }
            });
        }

        public async Task<AuthResultDTO> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var user = await _authRepository.GetUserByEmailAsync(dto.Email);

            if (user == null ||
                user.PasswordResetToken != dto.Token ||
                user.PasswordResetTokenExpiry == null ||
                user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                _logger.LogWarning("Tentativo reset password con token invalido/scaduto per {Email}", dto.Email);
                return new AuthResultDTO
                {
                    Success = false,
                    Message = "Token non valido o scaduto"
                };
            }

            // Aggiorna password
            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _authRepository.UpdateUserAsync(user);

            _logger.LogInformation("Password reimpostata con successo per utente {UserId}", user.Id);

            return new AuthResultDTO
            {
                Success = true,
                Message = "Password reimpostata con successo"
            };
        }

        public async Task<AuthResultDTO> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _authRepository.GetRefreshTokenAsync(refreshToken);
            
            if (storedToken == null || !storedToken.IsActive)
            {
                _logger.LogWarning("Tentativo refresh con token invalido");
                return new AuthResultDTO
                {
                    Success = false,
                    Message = "Token non valido"
                };
            }

            var user = await _authRepository.GetByIdAsync(storedToken.UserId);

            if (user == null)
            {
                return new AuthResultDTO
                {
                    Success = false,
                    Message = "Utente non trovato"
                };
            }
            
            // Genera nuovi token
            string newAccessToken = _tokenService.GenerateToken(user);
            string newRefreshToken = GenerateRefreshToken();

            // Revoca vecchio refresh token
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.ReplacedByToken = newRefreshToken;
            await _authRepository.UpdateRefreshTokenAsync(storedToken);

            // Salva nuovo refresh token
            await SaveRefreshTokenAsync(user.Id, newRefreshToken);

            _logger.LogInformation("Token refreshed per utente {UserId}", user.Id);

            return new AuthResultDTO
            {
                Success = true,
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Message = "Token rinnovato con successo",
                Expiration = _tokenService.GetTokenExpiration(newAccessToken),
                User = new UserDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role
                }
            };
        }

        public async Task RevokeRefreshTokensAsync(int userId)
        {
            await _authRepository.RevokeAllRefreshTokensAsync(userId);
            _logger.LogInformation("Tutti i refresh token revocati per utente {UserId}", userId);
        }

        // ===== HELPER METHODS =====

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string GenerateRandomToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private async Task SaveRefreshTokenAsync(int userId, string token)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.SaveRefreshTokenAsync(refreshToken);
        }
    }
}
