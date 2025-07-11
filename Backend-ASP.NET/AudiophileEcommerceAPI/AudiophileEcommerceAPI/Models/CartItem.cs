namespace AudiophileEcommerceAPI.Models
{
    public class CartItem
    {
        public int Id{ get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; } = new Cart();
        public int ProductId { get; set; }
        public Product Product { get; set; } = new Product();
        public int Quantity { get; set; }
    }
}
