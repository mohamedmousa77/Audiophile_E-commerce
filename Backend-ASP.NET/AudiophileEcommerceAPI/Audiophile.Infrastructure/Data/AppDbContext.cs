/*
 Responsabilità: Questo file definisce il "ponte" tra il tuo codice C# e il tuo database SQL.

   In dettaglio:

Eredita da DbContext (di Entity Framework Core)

Contiene le DbSet<T> per ogni entità del tuo dominio (Product, Order, ecc.)

Si occupa di mappare le classi .cs (modelli) alle tabelle nel database
*/

using Microsoft.EntityFrameworkCore;
using Audiophile.Domain.Models;

namespace Audiophile.Infrastructure.Data
{
    public class AppDbContext : DbContext

    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<CustomerInfo> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .Property(o => o.Shipping)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Subtotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Total)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.VAT)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(i => i.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CustomerInfo>()
                .HasOne(c => c.Cart)
                .WithOne()
                .HasForeignKey<Cart>(c => c.CustomerInfoId);
        }

    }
}
