using Audiophile.Application.DTOs.Auth;
using Audiophile.Application.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AudiophileEcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration config, IAuthService authService)
        {
            _config= config;
            _authService= authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResultDTO), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.RegisterAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                // Cattura errori di business (es. "Utente già registrato")
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Si è verificato un errore durante la registrazione.");
            }  
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResultDTO), 200)]
        [ProducesResponseType(400)]

        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.LoginAsync(dto);

                if (!result.Success)
                    return Unauthorized(new { message = result.Message });

                return Ok(result);
            }
            catch (ArgumentException)
            {
                return BadRequest(new { message = "Email o Password non valide." });
            }
            catch (Exception)
            {
                return StatusCode(500, "Si è verificato un errore durante l'accesso.");
            }


        }
    }
}
