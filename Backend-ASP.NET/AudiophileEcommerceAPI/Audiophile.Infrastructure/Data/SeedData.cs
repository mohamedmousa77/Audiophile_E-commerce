/*
 Responsabilità: Inizializza il database con dati di esempio (prodotti, categorie, ecc.) se è vuoto.

💡 Serve soprattutto in ambienti di test o demo per evitare un database vuoto.
 */

using AudiophileEcommerceAPI.Models;

namespace AudiophileEcommerceAPI.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product { Name = "XX99 Mark II Headphones", Price = 299.99M, Category = "Headphones" },
                    new Product { Name = "ZX9 Speaker", Price = 599.99M, Category = "Speakers" }
                );

                context.SaveChanges();
            }
        }
    }
}
