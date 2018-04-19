using System;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var connectionString = $"Server={dbHost};Database=MyStore;Trusted_Connection=True;MultipleActiveResultSets=true";
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
        }
    }
    
    //dotnet ef migrations add NAME
    //dotnet ef database update
}