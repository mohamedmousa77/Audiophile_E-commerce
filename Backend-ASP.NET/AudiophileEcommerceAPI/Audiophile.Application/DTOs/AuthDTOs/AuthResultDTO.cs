

namespace Audiophile.Application.DTOs.Auth
{
    public class AuthResultDTO
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? Expiration {  get; set; }
        public string? Message { get; set; }
        public UserDTO? User { get; set; }
        

    }
    public class UserDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}
