using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStore.Domain;
using MyStore.Domain.Repositories;

namespace MyStore.Infrastructure.EF
{
    public class EfProductRepository : IProductRepository
    {
        private readonly MyStoreContext _context;

        public EfProductRepository(MyStoreContext context)
        {
            _context = context;
        }

        public async Task<Product> GetAsync(Guid id)
            => await _context.Products.Include(x=>x.Files).SingleOrDefaultAsync(p => p.Id == id);

        public async Task<IQueryable<Product>> BrowseAsync(string name)
        {
            var products = _context.Products.Include(x => x.Files).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(name))
            {
                products = products.Where(x => x.Name.Contains(name));
            }

            return products;
        }

        public async Task<IQueryable<Product>> BrowseByUserId(string name, int? pageIndex, Guid userId)
        {
            if (userId != new Guid("00000000-0000-0000-0000-000000000000"))
            {
                var products = _context.Products.Where(x => x.UserId == userId).Include(x => x.Files).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    products = products.Where(x => x.Name.Contains(name));
                }
                return products;
            }
            else
            {
                var products = _context.Products.Include(x => x.Files).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    products = products.Where(x => x.Name.Contains(name));
                }
                return products;
            }
        }

        public async Task CreateAsync(Product product)
        {
            try
            {
                await _context.Products.AddAsync(product);

                var files = _context.Files.Where(c => c.ProductId == null && c.UserId == product.UserId);
                await files.ForEachAsync(b => b.ProductId = product.Id);

            }
            catch
            {
                ;
            }
            finally
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteImageFromProduct(Guid productId, Guid imageId, Guid userId)
        {
            var image = _context.Files.Where(x => x.ProductId == productId && x.UserId == userId && x.Id == imageId).FirstOrDefault();
            _context.Files.Remove(image);
            _context.SaveChanges();

            var filesPath = Environment.GetEnvironmentVariable("UPLOAD_DIR");
            File.Delete(Path.Combine(filesPath, imageId.ToString()));
            File.Delete(Path.Combine(filesPath, "min_" + imageId));
        }

        public async Task DeleteImage(Guid imageId, Guid userId)
        {
            var image = _context.Files.Where(x => x.UserId == userId && x.Id == imageId).FirstOrDefault();
            _context.Files.Remove(image);
            _context.SaveChanges();

            var filesPath = Environment.GetEnvironmentVariable("UPLOAD_DIR");
            File.Delete(Path.Combine(filesPath, imageId.ToString()));
            File.Delete(Path.Combine(filesPath, "min_" + imageId));
        }

        public async Task UpdateProduct(Guid id, string name, decimal price, string category, string description)
        {
           var product = _context.Products.Where(x => x.Id == id).FirstOrDefault();
            product.Name = name;
            product.Price = price;
            product.Description = description;
            product.Category = category;

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }

}