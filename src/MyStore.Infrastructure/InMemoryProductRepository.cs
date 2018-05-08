using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyStore.Domain;
using MyStore.Domain.Repositories;

namespace MyStore.Infrastructure
{
    public class InMemoryProductRepository : IProductRepository
    {
        //No thread safe
        private readonly List<Product> _products = new List<Product>();

        public async Task<Product> GetAsync(Guid id)
            => await Task.FromResult(_products
                .SingleOrDefault(p => p.Id == id));

        public async Task<IEnumerable<Product>> BrowseAsync(string name)
        {
            await Task.CompletedTask;
            if (!string.IsNullOrWhiteSpace(name))
            {
                return _products.Where(x => x.Name.Contains(name));
            }

            return _products;
        }

        public async Task CreateAsync(Product product)
        {
            await Task.CompletedTask;
            _products.Add(product);

        }
    }
}