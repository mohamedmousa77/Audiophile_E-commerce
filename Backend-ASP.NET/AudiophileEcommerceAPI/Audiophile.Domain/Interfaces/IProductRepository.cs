
using Audiophile.Domain.Models;

namespace Audiophile.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product?> GetProductByIdWithLockAsync(int id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetByCategory(string category);
        Task<IEnumerable<Product>> GetFilteredProductsAsync(bool? isPromotion, bool? isNew);
        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);

    }
}
