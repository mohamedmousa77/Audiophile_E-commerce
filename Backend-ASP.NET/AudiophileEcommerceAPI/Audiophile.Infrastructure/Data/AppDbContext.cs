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
        public DbSet<CustomerInfo> CustomerInfos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== GLOBAL QUERY FILTER PER SOFT DELETE =====
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            //modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);



            // ===== CONFIGURAZIONE USER (Autenticazione) =====
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20);
            });

            // ===== CONFIGURAZIONE CUSTOMERINFO (Dati Spedizione) =====
            modelBuilder.Entity<CustomerInfo>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ===== CONFIGURAZIONE ORDER =====
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(o => o.Shipping).HasColumnType("decimal(18,2)");
                entity.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(o => o.Total).HasColumnType("decimal(18,2)");
                entity.Property(o => o.VAT).HasColumnType("decimal(18,2)");

                // Relazione con CustomerInfo
                entity.HasOne<CustomerInfo>()
                    .WithMany(ci => ci.Orders)
                    .HasForeignKey("CustomerInfoId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== CONFIGURAZIONE ORDERITEM =====
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");

                // Relazione con Order
                entity.HasOne<Order>()
                    .WithMany()
                    .HasForeignKey("OrderId")
                    .OnDelete(DeleteBehavior.Cascade);

                // Relazione con Product
                entity.HasOne<Product>()
                    .WithMany()
                    .HasForeignKey("ProductId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== CONFIGURAZIONE PRODUCT =====
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });

            // ===== CONFIGURAZIONE CART =====
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.Id);
            });

            // ===== CONFIGURAZIONE CARTITEM =====
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);

                // Relazione con Cart
                entity.HasOne<Cart>()
                    .WithMany()
                    .HasForeignKey("CartId")
                    .OnDelete(DeleteBehavior.Cascade);

                // Relazione con Product
                entity.HasOne<Product>()
                    .WithMany()
                    .HasForeignKey("ProductId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

        }

    }
}
