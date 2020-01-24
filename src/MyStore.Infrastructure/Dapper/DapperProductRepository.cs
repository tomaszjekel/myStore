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