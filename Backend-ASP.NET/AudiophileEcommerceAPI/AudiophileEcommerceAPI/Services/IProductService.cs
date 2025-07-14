using AudiophileEcommerceAPI.DTOs;
using AudiophileEcommerceAPI.Models;

namespace AudiophileEcommerceAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product?> GetProductById(int id);
        Task<IEnumerable<Product>> GetByCategory(string category);

        Task<ProductDTO> CreateProduct(Product product);

        Task<bool> UpdateProduct(Product product);

        Task<bool> DeleteProduct(int id);
    }
}
