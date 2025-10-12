namespace Audiophile.Domain.Models
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerInfoId { get; set; }
        public CustomerInfo CustomerInfo { get; set; } = null!;

        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal VAT { get; set; }
        public decimal Total { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? CancellationReason { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
