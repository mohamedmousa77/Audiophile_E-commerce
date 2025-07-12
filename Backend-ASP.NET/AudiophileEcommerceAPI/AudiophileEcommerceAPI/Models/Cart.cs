namespace AudiophileEcommerceAPI.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int CustomerInfoId { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public List<CartItem> Items { get; set; }
    }
}
