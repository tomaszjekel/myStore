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

        public async Task<IQueryable<Product>> BrowseByUserId(string name, int? pageIndex, Guid userId, Guid? category)
        {
            if (userId != new Guid("00000000-0000-0000-0000-000000000000"))
            {
                var products = _context.Products.Where(x => x.UserId == userId && x.Deleted == false).Include(x => x.Files).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    products = products.Where(x => x.Name.Contains(name) && x.UserId == userId).Include(x => x.Files).AsNoTracking();
                }

                return products;
            }
            else
            {
                var products = _context.Products.Where(x=>x.Deleted==false).Include(x => x.Files).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    products = products.Where(x => x.Name.Contains(name)).Include(x => x.Files).AsNoTracking();
                }
                if (category != null)
                {
                    products = products.Where(x => x.Category == category.ToString());
                }
                return products;
            }
        }

        public async Task CreateAsync(Product product)
        {
            try
            {

                var files = _context.Files.Where(c => c.ProductId == null && c.UserId == product.UserId);
                product.Files = files.ToList();
                await _context.Products.AddAsync(product);

                await files.ForEachAsync(b => b.ProductId = product.Id);

                await _context.SaveChangesAsync();


            }
            catch(Exception ex) 
            {
                ;
            }
            finally
            {
                ;
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

        public async Task DeleteImage(string imageName, Guid userId)
        {
            var image = _context.Files.Where(x => x.UserId == userId && x.Name == imageName).FirstOrDefault();
            _context.Files.Remove(image);
            _context.SaveChanges();

            var filesPath = Environment.GetEnvironmentVariable("UPLOAD_DIR");
            File.Delete(Path.Combine(filesPath, imageName));
            File.Delete(Path.Combine(filesPath, "min_" + imageName));
        }

        public async Task UpdateProduct(Product p)
        {
           var product = _context.Products.Where(x => x.Id == p.Id).FirstOrDefault();
            product.Name = p.Name;
            product.Price = p.Price;
            product.Description = p.Description;
            product.Category = p.Category;
            product.CityId = p.CityId;

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public async Task DeleteProduct(Guid productId, Guid userId)
        {
            var product = _context.Products.Include(x=>x.Files).Where(x => x.Id == productId && x.UserId == userId).FirstOrDefault();
            product.Deleted= true;

            foreach(var name in product.Files)
            {
                var filesPath = Environment.GetEnvironmentVariable("UPLOAD_DIR");
                File.Delete(Path.Combine(filesPath, name.Name.ToString()));
                File.Delete(Path.Combine(filesPath, "min_" + name.Name));
            }
            try
            {
                //_context.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                ;
            }
        }
        public async Task<List<Cities>> GetCities()
        {
            return  _context.Cities.Select(x =>new Cities { Id = x.Id, Name = x.Name, Province_Id = x.Province_Id }).OrderBy(x=>x.Name).ToList();
                
        }
    }

}