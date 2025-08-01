using AudiophileEcommerceAPI.DTOs;
using AudiophileEcommerceAPI.Models;
using AudiophileEcommerceAPI.Services;
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
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterCustomer(dto);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { token = result.Token });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.AuthenticateCustomer (request.Email, request.Password);
            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}
