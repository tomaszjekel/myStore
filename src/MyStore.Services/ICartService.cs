using System;
using System.Threading.Tasks;

namespace MyStore.Services
{
    public interface ICartService
    {
        Task AddProductAsync(Guid userId, Guid productId);
        Task CreateAsync(Guid userId);
        Task DeleteAsync(Guid userId);
    }
}