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
        Task DeleteImageFromProduct(Guid productId, Guid imageId, Guid userId);
        Task DeleteImage( Guid imageId, Guid userId);
        Task UpdateProduct(Guid id, string name, decimal price, string category, string description);
        Task<IQueryable<Product>> BrowseByUserId(string name, int? pageIndex, Guid userId);
        Task DeleteProduct(Guid productId, Guid userId);
    }
}