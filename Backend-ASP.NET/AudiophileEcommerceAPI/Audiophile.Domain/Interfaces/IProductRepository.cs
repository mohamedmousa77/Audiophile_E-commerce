
using Audiophile.Domain.Models;

namespace Audiophile.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product?> GetProductById(int id);
        Task<IEnumerable<Product>> GetByCategory(string category);

        Task<Product> CreateProduct(Product product);
        Task<IEnumerable<Product>> GetFilteredProductsAsync(bool? isPromotion, bool? isNew);
        Task<bool> UpdateProduct(Product product);

        Task<bool> DeleteProduct(int id);
    }
}
