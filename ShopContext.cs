using System;
using System.IO;
using Databas2.Models;
using Microsoft.EntityFrameworkCore;

namespace Databas2
{
    // DBContext = "enheten" representerar databasen
    public class ShopContext : DbContext
    {
        // Db<Category> mappar till tabellen Category i databasen
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Book> Books => Set<Book>();

        // Configure to use SQLite file
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "Shop.db");
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }

        // Fine-tune model: set primary key and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(e =>
            {
                // Change x => x.Id if your PK property has another name (e.g. CategoryId)
                e.HasKey(x => x.CategoryId);
                
                // Säkerställer samma regler som data annotations (Required + MaxLenght)
                e.Property(x => x.CategoryName)
                    .IsRequired().HasMaxLength(100);
                
                e.Property(x => x.CategoryDescription).HasMaxLength(250);
                
                // skapar ett UNIQUE index i CategoryName
                e.HasIndex(x => x.CategoryName).IsUnique();
                
                // Kategori har flera produkter
                e.HasMany(c=>c.Products)
                    .WithOne(c=>c.Category)
                    .HasForeignKey(c=>c.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Produkt
            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(x => x.ProductId);
                e.Property(x => x.ProductName).IsRequired().HasMaxLength(100);
                e.Property(x => x.Price).IsRequired();
                e.Property(x => x.Description).HasMaxLength(250);
                //e.HasIndex(x => x.ProductName).IsUnique();
            });

            modelBuilder.Entity<Author>(e =>
            {
                e.HasKey(x => x.AuthorId);
                e.Property(x => x.AuthorName).IsRequired().HasMaxLength(100);
                e.Property(x => x.AuthorCountry).IsRequired().HasMaxLength(100);
                e.HasIndex(a=>a.AuthorName).IsUnique();
                
                e.HasMany(a => a.Books)
                    .WithOne(b => b.Author)
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Book>(e =>
                {
                    e.HasKey(x => x.BookId);
                    e.Property(x => x.BookTitle).IsRequired().HasMaxLength(100);
                    e.Property(x => x.ReleaseYear).IsRequired().HasMaxLength(100);
                    e.HasOne(x => x.Author)
                        .WithMany(x => x.Books)
                        .HasForeignKey(x => x.AuthorId)
                        .OnDelete(DeleteBehavior.Restrict);
                }
            );
        }
    }
}