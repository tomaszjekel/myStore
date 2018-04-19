using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MyStore.Domain;
using MyStore.Domain.Repositories;
using Dapper;

namespace MyStore.Infrastructure.Dapper
{
    public class DapperProductRepository : IProductRepository
    {
        public async Task<IEnumerable<Product>> BrowseAsync(string name)
        {
            using (var connection = new SqlConnection())
            {
                return await connection.QueryAsync<Product>("select * from Products");
            }
        }

        public async Task CreateAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<Product> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}