
using System.ComponentModel.DataAnnotations;

namespace Audiophile.Application.DTOs.AuthDTOs
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
