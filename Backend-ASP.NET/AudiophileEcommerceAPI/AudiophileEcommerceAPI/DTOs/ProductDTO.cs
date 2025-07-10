using System.ComponentModel.DataAnnotations;

namespace AudiophileEcommerceAPI.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The product name of the customer is necessary")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "The product price of the customer is necessary")]
        public decimal Price { get; set; } = decimal.Zero;
        [Required(ErrorMessage = "The product image of the customer is necessary")]
        public string ImageUrl { get; set; } = string.Empty;
        [Required(ErrorMessage = "The product country of the customer is necessary")]
        public string Category { get; set; } = string.Empty;

    }
}
