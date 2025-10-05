using System.ComponentModel.DataAnnotations;

namespace Audiophile.Application.DTOs
{
    public class OrderItemDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The product id of the customer is necessary")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "The quantity of the customer is necessary")]
        public int Quantity { get; set; }
    }
}
