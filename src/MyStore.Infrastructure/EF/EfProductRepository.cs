using System;
using System.Collections.Generic;
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
    }
}