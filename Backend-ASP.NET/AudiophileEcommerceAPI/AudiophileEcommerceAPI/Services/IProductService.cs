using AudiophileEcommerceAPI.Models;

namespace AudiophileEcommerceAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product?> GetProductById(int id);

        Task<Product> CreateProduct(Product product);

        Task<bool> UpdateProduct(Product product);

        Task<bool> DeleteProduct(int id);
    }
}
