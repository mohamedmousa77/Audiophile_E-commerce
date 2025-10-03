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
        [Required(ErrorMessage = "Product image is required")]
        public string ImageUrl { get; set; } = string.Empty;
        [Required(ErrorMessage = "The product category of the customer is necessary")]
        public string Category { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public bool? IsPromotion { get; set; }
        public bool? IsNew { get; set; }

    }
}
