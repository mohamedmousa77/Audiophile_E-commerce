using AudiophileEcommerceAPI.Data;
using AudiophileEcommerceAPI.Models;

namespace AudiophileEcommerceAPI.Services
{
    public class SeedDataRepository : ISeedDataRepository
    {
        private readonly AppDbContext _context;

        public SeedDataRepository(AppDbContext context)
        {
            _context = context;
        }
        public void Seed()
        {
            if (!_context.Products.Any())
            {
                _context.Products.AddRange(
                   new Product { Name = "XX99 Mark II Headphones", Price = 299.99M, Category = "Headphones" },
                   new Product { Name = "ZX9 Speaker", Price = 599.99M, Category = "Speakers" }
                );
                _context.SaveChanges();
            }

        }
    }
}
