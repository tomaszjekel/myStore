using System;
using System.Threading.Tasks;

namespace MyStore.Domain.Repositories
{
    public interface IProfileRepository
    {
        Task<Profile> GetAsync(Guid id);
        Task CreateAsync(Profile profile);
    }
}
