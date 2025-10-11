using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using Audiophile.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Audiophile.Infrastructure.Repositories
{
    public class ProductRepository : RepositoryBase, IProductRepository
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<ProductRepository> _logger;
        private const string CacheKeyPrefix = "product_";
        private const int CacheExpirationMinutes = 60;

        public ProductRepository(
            AppDbContext context,
            IDistributedCache cache,
            ILogger<ProductRepository> logger) : base(context)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            _logger.LogInformation("Product {ProductId} created", product.Id);
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await Context.Products.FindAsync(id);
            if (product == null)
                return false;

            // Soft delete
            product.IsDeleted = true;
            await Context.SaveChangesAsync();

            // Invalidate cache
            var cacheKey = $"{CacheKeyPrefix}{id}";
            await _cache.RemoveAsync(cacheKey);

            _logger.LogInformation("Product {ProductId} soft deleted", id);
            return true;

        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            var cacheKey = "product_categories";
            var cached = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<IEnumerable<string>>(cached) ?? Enumerable.Empty<string>();
            }

            var categories = await Context.Products
                .Where(p => !p.IsDeleted)
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            var serialized = JsonSerializer.Serialize(categories);
            await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            });

            return categories;
        }


        public async Task<IEnumerable<Product>> GetAllProductsAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? searchTerm = null)
        {
            var query = Context.Products.AsQueryable();

            // Filtri
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
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
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm));
            }

            // Pagination
            return await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {


            // Try cache first
            var cacheKey = $"{CacheKeyPrefix}{id}";
            var cachedProduct = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedProduct))
            {
                _logger.LogInformation("Product {ProductId} retrieved from cache", id);
                return JsonSerializer.Deserialize<Product>(cachedProduct);
            }

            // Not in cache, get from database
            var product = await Context.Products.FindAsync(id);

            if (product != null)
            {
                // Cache it
                var serialized = JsonSerializer.Serialize(product);
                await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
                });

                _logger.LogInformation("Product {ProductId} cached", id);
            }

            return product;
        }

        public async Task<Product?> GetProductByIdWithLockAsync(int id)
        {
            // Se hai più utenti che cercano di aggiornare lo stesso prodotto/stock
            // contemporaneamente, questo metodo evita che due transazioni leggano il
            // valore, lo modifichino e lo scrivano insieme causando dati inconsistenti
            // o errori tipo “oversell”.

            // Pessimistic lock per evitare race conditions sullo stock
            return await Context.Products
                .FromSqlRaw(@"
                    SELECT * FROM Products WITH (UPDLOCK, ROWLOCK) 
                    WHERE Id = {0}", id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetTotalProductCountAsync(
            string? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? searchTerm = null)
        {
            var query = Context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
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
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm));
            }

            return await query.CountAsync();
        }


        public async Task UpdateProductAsync(Product product)
        {
            Context.Products.Update(product);
            await Context.SaveChangesAsync();

            // Invalidate cache
            var cacheKey = $"{CacheKeyPrefix}{product.Id}";
            await _cache.RemoveAsync(cacheKey);

            _logger.LogInformation("Product {ProductId} updated and cache invalidated", product.Id);

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
