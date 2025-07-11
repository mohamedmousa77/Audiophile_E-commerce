/*
 Responsabilità: Questo file definisce il "ponte" tra il tuo codice C# e il tuo database SQL.

   In dettaglio:

Eredita da DbContext (di Entity Framework Core)

Contiene le DbSet<T> per ogni entità del tuo dominio (Product, Order, ecc.)

Si occupa di mappare le classi .cs (modelli) alle tabelle nel database
*/

using AudiophileEcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AudiophileEcommerceAPI.Data
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

            modelBuilder.Entity<CustomerInfo>()
                .HasOne(c => c.Cart)
                .WithOne(c => c.CustomerInfo)
                .HasForeignKey<Cart>(c => c.CustomerId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);
        }

    }
}
