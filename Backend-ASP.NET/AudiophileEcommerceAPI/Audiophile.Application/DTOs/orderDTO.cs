using System.ComponentModel.DataAnnotations;

namespace Audiophile.Application.DTOs
{
    public class OrderDTO
    {
        // Customer Info
        [Required(ErrorMessage = "The name of the customer is necessary")][StringLength(100, ErrorMessage = "The customer name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The email of the customer is necessary")][StringLength(100, ErrorMessage = "The customer email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "The phone of the customer is necessary")][StringLength(100, ErrorMessage = "The customer phone cannot exceed 100 characters")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Address of the customer is necessary")][StringLength(100, ErrorMessage = " customer address cannot exceed 100 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "The city of the customer is necessary")][StringLength(100, ErrorMessage = " customer city cannot exceed 100 characters")] 
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "The country of the customer is necessary")][StringLength(100, ErrorMessage = " customer country cannot exceed 100 characters")] 
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Zip Code of the customer is necessary")][StringLength(100, ErrorMessage = " customer zip code cannot exceed 100 characters")]
        public string ZipCode { get; set; } = string.Empty;
        public string Status {  get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }


        [Required(ErrorMessage = "The items of cart is necessary")]
        // Cart
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }
}
