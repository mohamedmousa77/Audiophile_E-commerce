// This File contains the business logic and conversion from DTO to Entity (Models)

using Audiophile.Application.DTOs;
using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Audiophile.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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
                Price = dto.Price,
                Category = dto.Category,
                Description = "Prodotto standard",
                StockQuantity = 100,
                IsNew = dto.IsNew, 
                IsPromotion = dto.IsPromotion
            };

            var createdProduct = await _productRepository.CreateProductAsync(productEntity);

            var readDto = new ProductReadDTO
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Price = createdProduct.Price,
                Category = createdProduct.Category,
                IsInStock = createdProduct.StockQuantity > 0,
                IsNew = createdProduct.IsNew,
                IsPromotion = createdProduct.IsPromotion
            };

            return readDto;
        }

        public async Task<ProductReadDTO?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return null;

            return new ProductReadDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Category = product.Category,
                IsInStock = product.StockQuantity > 0,
                IsNew = product.IsNew, 
                IsPromotion = product.IsPromotion
            };
        }

        public async Task<IActionResult> UpdateProductAsync(int id ,ProductUpdateDTO dto)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(dto.Id);
            if (existingProduct == null) return false;


            if (dto.Name != null) existingProduct.Name = dto.Name;
            if (dto.Description != null) existingProduct.Description = dto.Description;
            existingProduct.Price = dto.Price;
            existingProduct.ImageUrl = dto.ImageUrl;
            existingProduct.Category = dto.Category;
            existingProduct.StockQuantity = dto.StockQuantity;
            existingProduct.IsNew = dto.IsNew ?? false;
            existingProduct.IsPromotion = dto.IsPromotion ?? false;

            return await _productRepository.UpdateProductAsync(existingProduct);
        }

        public async Task<IEnumerable<Product>> GetByCategory(string category)
        {
            return await _productRepository.GetByCategory(category);
        }

        public async Task<IEnumerable<ProductReadDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.Select(p => new ProductReadDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Category = p.Category,
                IsInStock = p.StockQuantity > 0
            }).ToList();

        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteProductAsync(id);  
        }

        public async Task<IEnumerable<ProductReadDTO>> GetFilteredProductsAsync(bool? isPromotion, bool? isNew)
        {
            var products = await _productRepository.GetFilteredProductsAsync(isPromotion, isNew);

            return products.Select(p => new ProductReadDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Category = p.Category,
                IsInStock = p.StockQuantity > 0,
                IsNew = p.IsNew,
                IsPromotion = p.IsPromotion
            }).ToList();
        }
    }
}
