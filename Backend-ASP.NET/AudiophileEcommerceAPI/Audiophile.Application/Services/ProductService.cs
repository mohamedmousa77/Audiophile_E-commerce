// This File contains the business logic and conversion from DTO to Entity (Models)

using Audiophile.Application.DTOs;
using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Audiophile.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ProductReadDTO> CreateProductAsync(ProductCreateDTO dto)
        {
            if (dto.Price <= 0)
            {
                throw new ArgumentException("Il prezzo deve essere positivo.");
            }

            var productEntity = new Product
            {
                Name = dto.Name,
                Description = dto.Description ?? "Prodotto standard",
                Price = dto.Price,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                StockQuantity = dto.StockQuantity,
                IsNew = dto.IsNew,
                IsPromotion = dto.IsPromotion,
                CreatedAt = DateTime.UtcNow
            };

            var createdProduct = await _productRepository.CreateProductAsync(productEntity);

            _logger.LogInformation("Product created: {ProductId} - {Name}", createdProduct.Id, createdProduct.Name);

            return MapToReadDTO(createdProduct);
        }

        public async Task<ProductReadDTO?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found: {ProductId}", id);
                return null;
            }

            return MapToReadDTO(product);
        }

        public async Task<ProductReadDTO?> UpdateProductAsync(int id, ProductUpdateDTO dto)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(dto.Id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Attempt to update non-existent product: {ProductId}", id);
                return null;
            }


            if (!string.IsNullOrEmpty(dto.Name))
                existingProduct.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Description))
                existingProduct.Description = dto.Description;

            if (dto.Price > 0)
                existingProduct.Price = dto.Price;

            if (!string.IsNullOrEmpty(dto.ImageUrl))
                existingProduct.ImageUrl = dto.ImageUrl;

            if (!string.IsNullOrEmpty(dto.Category))
                existingProduct.Category = dto.Category;

            if (dto.StockQuantity >= 0)
                existingProduct.StockQuantity = dto.StockQuantity;

            if (dto.IsNew.HasValue)
                existingProduct.IsNew = dto.IsNew.Value;

            if (dto.IsPromotion.HasValue)
                existingProduct.IsPromotion = dto.IsPromotion.Value;

            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateProductAsync(existingProduct);

            _logger.LogInformation("Product updated: {ProductId}", id);

            return MapToReadDTO(existingProduct);
        }

        public async Task<IEnumerable<Product>> GetByCategory(string category)
        {
            return await _productRepository.GetByCategory(category);
        }

        public async Task<PagedResult<ProductReadDTO>> GetAllProductsAsync(
            int pageNumber,
            int pageSize,
            string? category,
            decimal? minPrice,
            decimal? maxPrice,
            string? searchTerm,
            string sortBy,
            string sortOrder)
        {
            var allProducts = await _productRepository.GetAllProductsAsync();

            var query = allProducts.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    p.Description.ToLower().Contains(searchLower));
            }

            // Ordinamento
            query = sortBy.ToLower() switch
            {
                "price" => sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),

                "date" => sortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.CreatedAt) 
                    : query.OrderBy(p => p.CreatedAt),
                    _ => sortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name)
            };
            var totalCount = query.Count();

            // Paginazione
            var products = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            _logger.LogInformation(
                "Retrieved {Count} products. Page: {Page}, Size: {Size}",
                products.Count, pageNumber, pageSize);

            return new PagedResult<ProductReadDTO>
            {
                Items = products.Select(MapToReadDTO),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var result=  await _productRepository.DeleteProductAsync(id);

            if (result)
            {
                _logger.LogInformation("Product deleted (soft delete): {ProductId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete product: {ProductId}", id);
            }

            return result;
        }

        public async Task<IEnumerable<ProductReadDTO>> GetFilteredProductsAsync(bool? isPromotion, bool? isNew)
        {
            var products = await _productRepository.GetFilteredProductsAsync(isPromotion, isNew);

            return products.Select(MapToReadDTO).ToList();
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }


        // ===== HELPER METHOD =====
        private ProductReadDTO MapToReadDTO(Product product)
        {
            return new ProductReadDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Category = product.Category,
                StockQuantity = product.StockQuantity,
                IsInStock = product.StockQuantity > 0,
                IsNew = product.IsNew,
                IsPromotion = product.IsPromotion
            };
        }
    }
}
