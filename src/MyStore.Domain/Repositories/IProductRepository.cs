using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStore.Domain.Repositories
{
    public interface IProductRepository
    {
        //CQS - Command & Query separation
        Task<Product> GetAsync(Guid id);
        Task<IQueryable<Product>> BrowseAsync(string name);
        Task CreateAsync(Product product);

    }
}