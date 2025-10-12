

using Audiophile.Application.DTOs;
using Audiophile.Domain.Models;

namespace Audiophile.Application.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductReadDTO>> GetAllProductsAsync(
           int pageNumber,
           int pageSize,
           string? category,
           decimal? minPrice,
           decimal? maxPrice,
           string? searchTerm,
           string sortBy,
           string sortOrder);

        Task<ProductReadDTO?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetByCategory(string category);
        Task<ProductReadDTO> CreateProductAsync(ProductCreateDTO dto);
        Task<ProductReadDTO?> UpdateProductAsync(int id, ProductUpdateDTO dto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<ProductReadDTO>> GetFilteredProductsAsync(bool? isPromotion, bool? isNew);
        Task<IEnumerable<string>> GetCategoriesAsync();
    }
}
