namespace AudiophileEcommerceAPI.Models
{
    public class OrderItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UniPrice { get; set; }
    }
}
