

namespace Audiophile.Application.DTOs
{
    public class CartDTOs
    {
        public class CartItemUpdateDTO
        {
            public int CustomerId { get; set; }
            public int ProductID { get; set; }
            public int Quantity { get; set; }
        }

        public class CartItemReadDTO
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public decimal UnitPrice { get; set; } 
            public decimal ItemTotal => Quantity * UnitPrice;
        }

        public class CartReadDTO
        {
            public int CartId { get; set; }
            public List<CartItemReadDTO> Items { get; set; } = new List<CartItemReadDTO>();
            public decimal TotalAmount { get; set; }
        }
    }
}
