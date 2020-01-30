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

        Task<Product> IProductRepository.GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<IQueryable<Product>> IProductRepository.BrowseAsync(string name)
        {
            throw new NotImplementedException();
        }

        Task IProductRepository.CreateAsync(Product product)
        {
            throw new NotImplementedException();
        }

        Task IProductRepository.DeleteImageFromProduct(Guid productId, Guid imageId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteImage(Guid imageId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProduct(Guid id, string name, decimal price, string category, string description)
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
    }
}