using System.ComponentModel.DataAnnotations;

namespace Audiophile.Application.DTOs.AuthDTOs
{
    public class RefreshTokenDTO
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
