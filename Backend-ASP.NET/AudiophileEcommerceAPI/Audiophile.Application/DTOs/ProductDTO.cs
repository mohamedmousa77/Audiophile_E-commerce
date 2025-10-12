using System.ComponentModel.DataAnnotations;

namespace Audiophile.Application.DTOs
{
    //public class ProductDTO
    //{
    //    [Required(ErrorMessage = "The product name of the customer is necessary")]
    //    public string Name { get; set; } = string.Empty;
    //    [Required(ErrorMessage = "The product price of the customer is necessary")]
    //    public decimal Price { get; set; } = decimal.Zero;
    //    [Required(ErrorMessage = "The product category of the customer is necessary")]
    //    public string Category { get; set; } = string.Empty;
    //    public bool? IsPromotion { get; set; }
    //    public bool? IsNew { get; set; }
    //}

    public class ProductUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public bool? IsNew { get; set; }
        public bool? IsPromotion { get; set; }
    }


    public class ProductCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsNew { get; set; } = false; 
        public bool IsPromotion { get; set; } = false;
    }

    // DTO per la lettura/output
    public class ProductReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsInStock { get; set; }
        public bool IsNew { get; set; }
        public bool IsPromotion { get; set; }

    }

}
