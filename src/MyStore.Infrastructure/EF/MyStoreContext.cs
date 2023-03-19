using System;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyStore.Domain;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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
        public DbSet<Cities> Cities { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Addresses> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }




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
            //GuidFunctions.Register(builder);
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
//public static class GuidFunctions
//{
//    public static bool IsGreaterThan(this Guid left, Guid right) => left.CompareTo(right) > 0;
//    public static bool IsGreaterThanOrEqual(this Guid left, Guid right) => left.CompareTo(right) >= 0;
//    public static bool IsLessThan(this Guid left, Guid right) => left.CompareTo(right) < 0;
//    public static bool IsLessThanOrEqual(this Guid left, Guid right) => left.CompareTo(right) <= 0;
//    public static void Register(ModelBuilder modelBuilder)
//    {
//        RegisterFunction(modelBuilder, nameof(IsGreaterThan), ExpressionType.GreaterThan);
//        RegisterFunction(modelBuilder, nameof(IsGreaterThanOrEqual), ExpressionType.GreaterThanOrEqual);
//        RegisterFunction(modelBuilder, nameof(IsLessThan), ExpressionType.LessThan);
//        RegisterFunction(modelBuilder, nameof(IsLessThanOrEqual), ExpressionType.LessThanOrEqual);
//    }
//    static void RegisterFunction(ModelBuilder modelBuilder, string name, ExpressionType type)
//    {
//        var method = typeof(GuidFunctions).GetMethod(name, new[] { typeof(Guid), typeof(Guid) });
//        modelBuilder.HasDbFunction(method).HasTranslation(parameters =>
//        {
//            var left = parameters.ElementAt(0);
//            var right = parameters.ElementAt(1);
//             return new SqlBinaryExpression(type, left, right, typeof(bool), null);
//        });
//    }
//}