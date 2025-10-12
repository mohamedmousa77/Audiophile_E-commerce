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
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== GLOBAL QUERY FILTER PER SOFT DELETE =====
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
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

                // Soft Delete
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                // Email Confirmation
                entity.Property(e => e.EmailConfirmed).HasDefaultValue(false);
                entity.Property(e => e.EmailConfirmationToken).HasMaxLength(500);

                // Password Reset
                entity.Property(e => e.PasswordResetToken).HasMaxLength(500);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);

                // Index sul token per ricerca veloce
                entity.HasIndex(rt => rt.Token).IsUnique();

                entity.Property(rt => rt.Token)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(rt => rt.CreatedByIp)
                    .HasMaxLength(50);

                entity.Property(rt => rt.RevokedByIp)
                    .HasMaxLength(50);

                entity.Property(rt => rt.ReplacedByToken)
                    .HasMaxLength(500);

                // Relazione con User
                entity.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);  // Se elimini user, elimina anche i suoi token
            });

            // ===== CONFIGURAZIONE CUSTOMERINFO (Dati Spedizione) =====
            modelBuilder.Entity<CustomerInfo>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(c => c.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== CONFIGURAZIONE ORDER =====
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.Shipping).HasColumnType("decimal(18,2)");
                entity.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(o => o.Total).HasColumnType("decimal(18,2)");
                entity.Property(o => o.VAT).HasColumnType("decimal(18,2)");

                // Status come string (enum)
                entity.Property(o => o.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(o => o.CancellationReason)
                    .HasMaxLength(500);

                // Relazione con CustomerInfo
                entity.HasOne(o => o.CustomerInfo)
                    .WithMany(ci => ci.Orders)
                    .HasForeignKey(o => o.CustomerInfoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relazione con Items
                entity.HasMany(o => o.Items)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== CONFIGURAZIONE ORDERITEM =====
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);

                entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");

                // Relazione con Product
                entity.HasOne(oi => oi.Product)
                    .WithMany()
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== CONFIGURAZIONE PRODUCT =====
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Description)
                    .HasMaxLength(2000);

                entity.Property(p => p.ImageUrl)
                    .HasMaxLength(500);

                entity.Property(p => p.Category)
                    .IsRequired()
                    .HasMaxLength(100);

                // Soft Delete
                entity.Property(p => p.IsDeleted).HasDefaultValue(false);

                // Indexes per performance
                entity.HasIndex(p => p.Category);
                entity.HasIndex(p => p.IsNew);
                entity.HasIndex(p => p.IsPromotion);
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
