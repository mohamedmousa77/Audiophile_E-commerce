using AudiophileEcommerceAPI.Data;
using AudiophileEcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AudiophileEcommerceAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _appDbContext;
        public ProductService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Product> CreateProduct(Product product)
        {
            _appDbContext.Products.Add(product);
            await _appDbContext.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _appDbContext.Products.FindAsync(id);
            if (product == null)  return false;
            
            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();
            return true;

        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _appDbContext.Products.ToListAsync();
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _appDbContext.Products.FindAsync(id);
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var existingProduct = await _appDbContext.Products.FindAsync(product.Id);
            if (existingProduct == null) return false;

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.Category = product.Category;
            existingProduct.StockQuantity = product.StockQuantity;

            await _appDbContext.SaveChangesAsync();
            return true;
        }
    }
}
