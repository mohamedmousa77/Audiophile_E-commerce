using Asp.Versioning;
using Audiophile.Application.DTOs.Auth;
using Audiophile.Application.DTOs.AuthDTOs;
using Audiophile.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace AudiophileEcommerceAPI.Controllers
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        //private readonly IConfiguration _config;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;


        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService= authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(typeof(AuthResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", registerDTO.Email);

            var result = await _authService.RegisterAsync(registerDTO);

            if (!result.Success)
            {
                _logger.LogWarning("Registration failed for email: {Email}. Reason: {Reason}",
                    registerDTO.Email, result.Message);
                return BadRequest(new { message = result.Message });
            }

            _logger.LogInformation("User registered successfully: {Email}", registerDTO.Email);
            return Ok(result);
        }

        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(typeof(AuthResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {

            _logger.LogInformation("Login attempt for email:{Email}", loginDto.Email);
            
            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                _logger.LogWarning("Login failed for email: {Email}", loginDto.Email);
                return Unauthorized(new { message = result.Message });
            }
            _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        [EnableRateLimiting("api")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Invalid user ID in token");
                return Unauthorized();
            }

            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return NotFound(new { message = "Utente non trovato" });
            }

            return Ok(user);
        }

        [HttpPost("forgot-password")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            _logger.LogInformation("Password reset requested for email: {Email}", dto.Email);
            await _authService.RequestPasswordResetAsync(dto.Email);

            // Sempre ritorna 200 per sicurezza (non rivelare se email esiste)
            return Ok(new { message = "Se l'email esiste, riceverai le istruzioni per il reset" });
        }

        [HttpPost("reset-password")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            _logger.LogInformation("Password reset attempt with token");

            var result = await _authService.ResetPasswordAsync(dto);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = "Password reimpostata con successo" });
        }

        [HttpPost("refresh-token")]
        [EnableRateLimiting("api")]
        [ProducesResponseType(typeof(AuthResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO dto)
        {
            _logger.LogInformation("Token refresh requested");

            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);

            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        [EnableRateLimiting("api")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                await _authService.RevokeRefreshTokensAsync(userId);
                _logger.LogInformation("User logged out: {UserId}", userId);
            }

            return Ok(new { message = "Logout effettuato con successo" });
        }
    }
}
}
