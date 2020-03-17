using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MyStore.Domain;
using MyStore.Domain.Repositories;
using Dapper;
using System.Linq;

namespace MyStore.Infrastructure.Dapper
{
    public class DapperProductRepository : IProductRepository
    {

        public async Task<IQueryable<Product>> BrowseAsync(string name)
        {
            using (var connection = new SqlConnection())
            {
                return connection.Query<Product>("select * from Products").AsQueryable() ;
            }
        }

        public Task<IQueryable<Product>> BrowseByUserId(string name, int? pageIndex, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task DeleteImage(string imageId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProduct(Guid productId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Product> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Cities>> GetCities()
        {
            throw new NotImplementedException();
        }

        public Task UpdateProduct(Guid id, string name, decimal price, string category, string description)
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

        Task<Product> IProductRepository.GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}