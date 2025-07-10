using System.ComponentModel.DataAnnotations;

namespace AudiophileEcommerceAPI.DTOs
{
    public class orderDTO
    {
        // Customer Info
        [Required(ErrorMessage = "The name of the customer is necessary")][StringLength(100, ErrorMessage = "Il nome del customer non può superare 100 caratteri")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "L'Email è obbligatoria")][StringLength(100, ErrorMessage = "L'email non può superare 100 caratteri")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Il nome del customer è obbligatoria")][StringLength(100, ErrorMessage = "Il nome del customer non può superare 100 caratteri")]
        public string Phone { get; set; } = string.Empty;
        [Required(ErrorMessage = "Il nome del customer è obbligatoria")]
        [StringLength(100, ErrorMessage = "Il nome del customer non può superare 100 caratteri")]
        public string Address { get; set; } = string.Empty;
        [Required(ErrorMessage = "Il nome del customer è obbligatoria")][StringLength(100, ErrorMessage = "Il nome del customer non può superare 100 caratteri")] 
        public string City { get; set; } = string.Empty;
        [Required(ErrorMessage = "Il nome del customer è obbligatoria")][StringLength(100, ErrorMessage = "Il nome del customer non può superare 100 caratteri")] 
        public string Country { get; set; } = string.Empty;
        [Required(ErrorMessage = "Il nome del customer è obbligatoria")][StringLength(100, ErrorMessage = "Il nome del customer non può superare 100 caratteri")]
        public string ZipCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Il nome del customer è obbligatoria")][StringLength(100, ErrorMessage = "Il nome del customer non può superare 100 caratteri")]
        // Cart
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }
}
