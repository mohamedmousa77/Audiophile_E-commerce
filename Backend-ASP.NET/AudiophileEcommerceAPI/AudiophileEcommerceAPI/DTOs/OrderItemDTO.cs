using System.ComponentModel.DataAnnotations;

namespace AudiophileEcommerceAPI.DTOs
{
    public class OrderItemDTO
    {
        [Required(ErrorMessage = "The product id of the customer is necessary")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "The quantity of the customer is necessary")]
        public int Quantity { get; set; }
    }
}
