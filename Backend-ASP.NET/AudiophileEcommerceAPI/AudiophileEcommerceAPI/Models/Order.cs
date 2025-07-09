namespace AudiophileEcommerceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerInfoId { get; set;}
        public CustomerInfo CustomerInfo { get; set; } = new CustomerInfo();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal VAT { get; set; }
        public decimal Total { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
