namespace Audiophile.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerInfoId { get; set;}
        public CustomerInfo CustomerInfo { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal VAT { get; set; }
        public decimal Total { get; set; }

        public enum Status { get, set, }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public ICollection<OrderItem> Items { get; set; }
    }
}
