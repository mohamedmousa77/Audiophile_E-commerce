/*
 Responsabilità: Inizializza il database con dati demo per testing (prodotti, categorie, ecc.) se è vuoto.

💡 Serve soprattutto in ambienti di test o demo per evitare un database vuoto.
 */

using Audiophile.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Audiophile.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task Initialize(AppDbContext context)
        {

            // ===== SEED USERS =====
            if (!await context.Users.AnyAsync())
            {
                var users = new[]
                {
                    new User
                    {
                        FullName = "Admin User",
                        Email = "admin@audiophile.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                        Role = "Admin",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        FullName = "Test Customer",
                        Email = "customer@test.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                        Role = "Customer",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();

                // ===== SEED PRODUCTS =====
                if (!await context.Products.AnyAsync())
                {
                    var products = new[]
                    {
                        new Product
                        {
                        Name = "XX99 Mark II Headphones",
                        Description = "The new XX99 Mark II headphones is the pinnacle of pristine audio. It redefines your premium headphone experience by reproducing the balanced depth and precision of studio-quality sound.",
                        Price = 2999m,
                        Category = "headphones",
                        StockQuantity = 50,
                        IsNew = true,
                        ImageUrl = "/assets/product-xx99-mark-two-headphones/mobile/image-product.jpg",
                        CreatedAt = DateTime.UtcNow
                        },
                        new Product
                        {
                        Name = "XX99 Mark I Headphones",
                        Description = "As the gold standard for headphones, the classic XX99 Mark I offers detailed and accurate audio reproduction for audiophiles.",
                        Price = 1750m,
                        Category = "headphones",
                        StockQuantity = 75,
                        IsNew = false,
                        ImageUrl = "/assets/product-xx99-mark-one-headphones/mobile/image-product.jpg",
                        CreatedAt = DateTime.UtcNow
                        },
                        new Product
                        {
                        Name = "XX59 Headphones",
                        Description = "Enjoy your audio almost anywhere and customize it to your specific tastes with the XX59 headphones. The stylish yet durable versatile wireless headset is a brilliant companion at home or on the move.",
                        Price = 899m,
                        Category = "headphones",
                        StockQuantity = 100,
                        IsNew = false,
                        ImageUrl = "/assets/product-xx59-headphones/mobile/image-product.jpg",
                        CreatedAt = DateTime.UtcNow
                        },
                        new Product
                        {
                            Name = "ZX9 Speaker",
                            Description = "Upgrade your sound system with the all new ZX9 active speaker. It's a bookshelf speaker system that offers truly wireless connectivity -- creating new possibilities for more pleasing and practical audio setups.",
                            Price = 4500m,
                            Category = "speakers",
                            StockQuantity = 30,
                            IsNew = true,
                            ImageUrl = "/assets/product-zx9-speaker/mobile/image-product.jpg",
                            CreatedAt = DateTime.UtcNow
                        },
                        new Product
                        {
                            Name = "ZX7 Speaker",
                            Description = "Stream high quality sound wirelessly with minimal loss. The ZX7 bookshelf speaker uses high-end audiophile components that represents the top of the line powered speakers for home or studio use.",
                            Price = 3500m,
                            Category = "speakers",
                            StockQuantity = 45,
                            IsNew = false,
                            ImageUrl = "/assets/product-zx7-speaker/mobile/image-product.jpg",
                            CreatedAt = DateTime.UtcNow
                        },
                        new Product
                        {
                            Name = "YX1 Wireless Earphones",
                            Description = "Tailor your listening experience with bespoke dynamic drivers from the new YX1 Wireless Earphones. Enjoy incredible high-fidelity sound even in noisy environments with its active noise cancellation feature.",
                            Price = 599m,
                            Category = "earphones",
                            StockQuantity = 150,
                            IsNew = true,
                            ImageUrl = "/assets/product-yx1-earphones/mobile/image-product.jpg",
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    await context.Products.AddRangeAsync(products);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
