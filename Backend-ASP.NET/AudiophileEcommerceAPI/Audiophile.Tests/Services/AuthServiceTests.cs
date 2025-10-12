using Audiophile.Application.DTOs.Auth;
using Audiophile.Application.Services;
using Audiophile.Application.Services.AuthServices;
using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Audiophile.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _authService;
        private readonly EmailService _emailServiceMock;

        public AuthServiceTests()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            _authService = new AuthService(
                _authRepositoryMock.Object,
                _tokenServiceMock.Object,
                _passwordHasherMock.Object,
                _loggerMock.Object,
                _emailServiceMock
            );
        }

        [Fact]
        public async Task RegisterAsync_WithNewEmail_ReturnsSuccess()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "Test@1234",
                Phone = "1234567890"
            };

            _authRepositoryMock
                .Setup(x => x.UserExistsByEmailAsync(registerDto.Email))
                .ReturnsAsync(false);

            _passwordHasherMock
                .Setup(x => x.HashPassword(registerDto.Password))
                .Returns("hashed_password");

            var user = new User
            {
                Id = 1,
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PasswordHash = "hashed_password",
                Role = "Customer"
            };

            _authRepositoryMock
                .Setup(x => x.RegisterAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            _tokenServiceMock
                .Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("jwt_token");

            _tokenServiceMock
                .Setup(x => x.GetTokenExpiration("jwt_token"))
                .Returns(DateTime.UtcNow.AddHours(1));

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("jwt_token", result.Token);
            Assert.NotNull(result.User);
            Assert.Equal(registerDto.Email, result.User.Email);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ReturnsFailure()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Email = "existing@example.com",
                Password = "Test@1234"
            };

            _authRepositoryMock
                .Setup(x => x.UserExistsByEmailAsync(registerDto.Email))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _authService.RegisterAsync(registerDto)
            );
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "test@example.com",
                Password = "Test@1234"
            };

            var user = new User
            {
                Id = 1,
                FullName = "Test User",
                Email = loginDto.Email,
                PasswordHash = "hashed_password",
                Role = "Customer"
            };

            _authRepositoryMock
                .Setup(x => x.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(x => x.VerifyPassword(loginDto.Password, user.PasswordHash))
                .Returns(true);

            _tokenServiceMock
                .Setup(x => x.GenerateToken(user))
                .Returns("jwt_token");

            _tokenServiceMock
                .Setup(x => x.GetTokenExpiration("jwt_token"))
                .Returns(DateTime.UtcNow.AddHours(1));

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("jwt_token", result.Token);
            Assert.NotNull(result.User);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ReturnsFailure()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var user = new User
            {
                Email = loginDto.Email,
                PasswordHash = "hashed_password"
            };

            _authRepositoryMock
                .Setup(x => x.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(x => x.VerifyPassword(loginDto.Password, user.PasswordHash))
                .Returns(false);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Email o password non corretti", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentEmail_ReturnsFailure()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "nonexistent@example.com",
                Password = "Test@1234"
            };

            _authRepositoryMock
                .Setup(x => x.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Email o password non corretti", result.Message);
        }
    }
}
