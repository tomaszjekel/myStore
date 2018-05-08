﻿using System;
using Microsoft.EntityFrameworkCore;
using MyStore.Domain;

namespace MyStore.Infrastructure.EF
{
    public class MyStoreContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FilesUpload> Files { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var connectionString = $"Server={dbHost};Database=MyStore;Trusted_Connection=True;MultipleActiveResultSets=true";
            //      var conn = "Server=tcp:tomodb.database.windows.net,1433;Initial Catalog=tomoDB;Persist Security Info=False;User ID={your_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
            optionsBuilder.UseSqlServer(connectionString, c => c.MigrationsAssembly("MyStore"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var orderItemBuilder = builder.Entity<OrderItem>();
            orderItemBuilder.Property(x => x.ProductId).IsRequired();
            orderItemBuilder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId);

            var orderBuilder = builder.Entity<Order>();
            orderBuilder.Property(x => x.UserId).IsRequired();
            orderBuilder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);

            var productBuilder = builder.Entity<Product>();
            productBuilder.Property(x => x.UserId).IsRequired();
            productBuilder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);

            var filesBuilder = builder.Entity<FilesUpload>();
            filesBuilder.Property(x => x.UserId).IsRequired();
            filesBuilder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);

        }
    }
    
    //dotnet ef migrations add NAME
    //dotnet ef database update
}