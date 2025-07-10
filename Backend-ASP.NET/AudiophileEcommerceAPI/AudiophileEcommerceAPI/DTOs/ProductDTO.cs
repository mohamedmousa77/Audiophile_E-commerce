namespace AudiophileEcommerceAPI.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } = decimal.Zero;
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

    }
}
