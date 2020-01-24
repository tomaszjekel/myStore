using System;
using Microsoft.EntityFrameworkCore;
using MyStore.Domain;

namespace MyStore.Infrastructure.EF
{
    public class MyStoreContext : DbContext
    {

        public MyStoreContext(DbContextOptions<MyStoreContext> options)
     : base(options)
        {
        }

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

            var DB_HOST = Environment.GetEnvironmentVariable("DB_HOST");
            var USER_NAME = Environment.GetEnvironmentVariable("USER_NAME");
            var PASS = Environment.GetEnvironmentVariable("PASS");
            var dbHost = "DESKTOP-60038OD\\SQLEXPRESS";
            var connectionString = $"Server={dbHost};Database=MyStore;Trusted_Connection=True;MultipleActiveResultSets=true";
            //var connectionString = $"Server={DB_HOST},1433;Initial Catalog=tomoDB;Persist Security Info=False;User ID={USER_NAME};Password={PASS};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //optionsBuilder.UseSqlServer(connectionString, c => c.MigrationsAssembly("MyStore"));
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