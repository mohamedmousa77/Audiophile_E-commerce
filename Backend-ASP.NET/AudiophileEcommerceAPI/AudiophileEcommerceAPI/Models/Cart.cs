namespace AudiophileEcommerceAPI.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public CustomerInfo CustomerInfo { get; set; } = new CustomerInfo();
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
