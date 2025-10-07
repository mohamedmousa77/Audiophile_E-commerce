using System.ComponentModel.DataAnnotations;

namespace Audiophile.Application.DTOs.Auth
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Il nome completo è obbligatorio")]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La password deve essere tra 8 e 100 caratteri")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "La password deve contenere almeno: 1 maiuscola, 1 minuscola, 1 numero, 1 carattere speciale")]
        public string Password { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Formato telefono non valido")]
        public string? Phone { get; set; }
    }
}
