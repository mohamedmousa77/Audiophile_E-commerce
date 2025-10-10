/*
 Responsabilità: Inizializza il database con dati demo per testing (prodotti, categorie, ecc.) se è vuoto.

💡 Serve soprattutto in ambienti di test o demo per evitare un database vuoto.
 */

using Audiophile.Domain.Models;
using Audiophile.Infrastructure.Data;

namespace AudiophileEcommerceAPI.Data
{
    public static class SeedData
    {
        public static async Task Initialize(AppDbContext dbContext)
        {
            if (!dbContext.Products.Any())
            {
                dbContext.Products.AddRange(
                    new Product { Name = "XX99 Mark II Headphones", Price = 299.99M, Category = "Headphones" },
                    new Product { Name = "ZX9 Speaker", Price = 599.99M, Category = "Speakers" }
                );

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
