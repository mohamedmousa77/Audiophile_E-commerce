using System.ComponentModel.DataAnnotations;


namespace Audiophile.Application.DTOs.Auth
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        public string Password { get; set; } = string.Empty;
    }
}
