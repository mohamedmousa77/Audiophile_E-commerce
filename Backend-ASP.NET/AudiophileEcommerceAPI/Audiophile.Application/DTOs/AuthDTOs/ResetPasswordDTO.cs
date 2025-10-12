

using System.ComponentModel.DataAnnotations;

namespace Audiophile.Application.DTOs.AuthDTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "La password deve contenere almeno: 1 maiuscola, 1 minuscola, 1 numero, 1 carattere speciale")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
