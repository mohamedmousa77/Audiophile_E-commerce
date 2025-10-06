using Audiophile.Infrastructure.Data;
using Audiophile.Domain.Models;
using Audiophile.Domain.Interfaces;


using Microsoft.EntityFrameworkCore;

namespace Audiophile.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _appDbContext;
        public ProductRepository(AppDbContext appDbContext)
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
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> GetByCategory(string category)
        {
            return await _appDbContext.Products
                .Where(p => p.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFilteredProductsAsync(bool? isPromotion, bool? isNew)
        {
            IQueryable<Product> query = _appDbContext.Products;

            if (isPromotion.HasValue)
                query = query.Where(p => p.IsPromotion == isPromotion.Value);

            if (isNew.HasValue)
                query = query.Where(p => p.IsNew == isNew.Value);

            // Solo qui viene eseguita la query sul DB!
            return await query.ToListAsync();
        }

    }
}
