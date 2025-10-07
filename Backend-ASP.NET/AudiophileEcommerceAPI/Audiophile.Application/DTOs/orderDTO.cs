
using System.ComponentModel.DataAnnotations;

namespace Audiophile.Application.DTOs
{
    public class OrderDTO
    {
        public class OrderCreateDTO
        {
            [Required(ErrorMessage = "Il nome completo è obbligatorio")]
            [StringLength(100, MinimumLength = 2)]
            public string CustomerFullName { get; set; } = string.Empty;
            [Required(ErrorMessage = "L'email è obbligatoria")]
            [EmailAddress]
            public string CustomerEmail { get; set; } = string.Empty;
            [Required(ErrorMessage = "Il telefono è obbligatorio")]
            [Phone]
            public string CustomerPhone { get; set; } = string.Empty;
            [Required(ErrorMessage = "L'indirizzo è obbligatorio")]
            public string CustomerAddress { get; set; } = string.Empty;
            [Required(ErrorMessage = "La città è obbligatoria")]
            public string CustomerCity { get; set; } = string.Empty;
            [Required(ErrorMessage = "Il paese è obbligatorio")]
            public string CustomerCountry { get; set; } = string.Empty;
            [Required(ErrorMessage = "Il codice postale è obbligatorio")]
            [RegularExpression(@"^\d{5}$", ErrorMessage = "Il codice postale deve essere di 5 cifre")]
            public string ZIPCode { get; set; } = string.Empty;
            [Required(ErrorMessage = "Almeno un prodotto è obbligatorio")]
            [MinLength(1, ErrorMessage = "Devi ordinare almeno un prodotto")]

            public List<CreateOrderItemDTO> OrderItems;

        }

        public class OrderReadDTO
        {
            public int Id { get; set; }
            public string CustomerFullName { get; set; } = string.Empty;
            public string CustomerEmail { get; set; } = string.Empty;
            public string CustomerPhone { get; set; } = string.Empty;
            public string CustomerAddress { get; set; } = string.Empty;
            public string CustomerCity { get; set; } = string.Empty;
            public string CustomerCountry { get; set; } = string.Empty;
            public string ZipCode { get; set; } = string.Empty;

            public decimal Subtotal { get; set; }
            public decimal Shipping { get; set; }
            public decimal VAT { get; set; }
            public decimal Total { get; set; }

            public string Status { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public List<ReadOrderItemDTO> OrderItems { get; set; } = new();
        }

        public class UpdateOrderDTO
        {
            [Required]
            public int OrderId { get; set; }
            [Required]
            [EmailAddress]
            public string? CustomerEmail { get; set; }
            public string? CustomerFullName { get; set;}            
            public string? CustomerPhone { get; set; } 
            public string? CustomerAddress { get; set; }
            public string? CustomerCity { get; set; }
            public string? CustomerCountry { get; set; }
            public string? ZIPCode { get; set; }

            public string? OrderStatus;
            public List<ReadOrderItemDTO>? OrderItems {  get; set; }

        }

        public class CreateOrderItemDTO {
            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "ProductId deve essere maggiore di 0")]

            public int ProductId { get; set; }
            [Required]
            [Range(1, 100, ErrorMessage = "La quantità deve essere tra 1 e 100")]
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }

        }

        public class ReadOrderItemDTO
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; } 

        }


    }
}
