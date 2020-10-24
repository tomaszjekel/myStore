using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyStore.Domain;
using MyStore.Domain.Repositories;

namespace MyStore.Infrastructure
{
    public class InMemoryProductRepository
    {
        //No thread safe
        private readonly List<Product> _products = new List<Product>();

        public async Task<Product> GetAsync(Guid id)
            => await Task.FromResult(_products
                .SingleOrDefault(p => p.Id == id));

        public async Task<IQueryable<Product>> BrowseAsync(string name)
        {
            await Task.CompletedTask;
            if (!string.IsNullOrWhiteSpace(name))
            {
                return _products.Where(x => x.Name.Contains(name)).AsQueryable();
            }

            return _products.AsQueryable();
        }

        public async Task CreateAsync(Product product)
        {
            await Task.CompletedTask;
            _products.Add(product);

        }

        public Task DeleteImage(string imageId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProduct(Product p)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Product>> BrowseByUserId(string name, int? pageIndex, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProduct(Guid productId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Cities>> GetCities()
        {
            throw new NotImplementedException();
        }

        public Task<List<Category>> GetCategories()
        {
            throw new NotImplementedException();
        }

        public void RemoveCategory(int id)
        {
            throw new NotImplementedException();
        }
    }
}