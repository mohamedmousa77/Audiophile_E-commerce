
namespace Audiophile.Application.DTOs
{
    public class OrderDTO
    {
        //// Customer Info
        //[Required(ErrorMessage = "The name of the customer is necessary")][StringLength(100, ErrorMessage = "The customer name cannot exceed 100 characters")]
        //public string FullName { get; set; } = string.Empty;

        //[Required(ErrorMessage = "The email of the customer is necessary")][StringLength(100, ErrorMessage = "The customer email cannot exceed 100 characters")]
        //public string Email { get; set; } = string.Empty;

        //[Required(ErrorMessage = "The phone of the customer is necessary")][StringLength(100, ErrorMessage = "The customer phone cannot exceed 100 characters")]
        //public string Phone { get; set; } = string.Empty;

        //[Required(ErrorMessage = "The Address of the customer is necessary")][StringLength(100, ErrorMessage = " customer address cannot exceed 100 characters")]
        //public string Address { get; set; } = string.Empty;

        //[Required(ErrorMessage = "The city of the customer is necessary")][StringLength(100, ErrorMessage = " customer city cannot exceed 100 characters")] 
        //public string City { get; set; } = string.Empty;

        //[Required(ErrorMessage = "The country of the customer is necessary")][StringLength(100, ErrorMessage = " customer country cannot exceed 100 characters")] 
        //public string Country { get; set; } = string.Empty;

        //[Required(ErrorMessage = "The Zip Code of the customer is necessary")][StringLength(100, ErrorMessage = " customer zip code cannot exceed 100 characters")]
        //public string ZipCode { get; set; } = string.Empty;
        //public string Status {  get; set; } = string.Empty;
        //public DateTime UpdatedAt { get; set; }


        //[Required(ErrorMessage = "The items of cart is necessary")]
        //// Cart
        //public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
        public class OrderCreateDTO
        {
            public string customerFullName { get; set; } = string.Empty;
            public string customerEmail { get; set; } = string.Empty;
            public string customerPhone { get; set; } = string.Empty;
            public string customerAddress { get; set; } = string.Empty;
            public string customerCity { get; set; } = string.Empty;
            public string customerCountry { get; set; } = string.Empty;
            public string ZIPCode { get; set; } = string.Empty;
            public List<CreateOrderItemDTO> orderItems;

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
            public string ZIPCode { get; set; } = string.Empty;
            public decimal totalOrderAmount { get; set; }
            public enum OrderStatus;
            public DateTime? UpdatedAt { get; set; }

            public List<ReadOrderItemDTO> OrderItems = new List<ReadOrderItemDTO>();
        }

        public class UpdateOrderDTO
        {

            public int OrderId { get; set; }
            public string? CustomerFullName { get; set;}
            public string? CustomerEmail { get; set; }
            public string? CustomerPhone { get; set; } 
            public string? CustomerAddress { get; set; }
            public string? CustomerCity { get; set; }
            public string? CustomerCountry { get; set; }
            public string? ZIPCode { get; set; }
            public decimal TotalOrderAmount { get; set; }


            public string? OrderStatus;

            public List<ReadOrderItemDTO>? OrderItems {  get; set; }

        }

        public class CreateOrderItemDTO {
            public int productId { get; set; }
            public int itemQuantity { get; set; }
            public decimal UnitPrice { get; set; }

        }

        public class ReadOrderItemDTO
        {
            public int id { get; set; }
            public int productId { get; set; }
            public int itemQuantity { get; set; }
            public decimal UnitPrice { get; set; }

        }


    }
}
