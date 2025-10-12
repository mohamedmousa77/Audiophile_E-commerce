
using Audiophile.Domain.Models;

namespace Audiophile.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(
            int pageNumber, int pageSize, string? category, decimal? minPrice,
            decimal? maxPrice,  string? searchTerm

            );
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetByCategory(string category);

        Task<Product> CreateProductAsync(Product product);
        Task<IEnumerable<Product>> GetFilteredProductsAsync(bool? isPromotion, bool? isNew);
        Task UpdateProductAsync(Product product);

        Task<bool> DeleteProductAsync(int id);
    }
}
