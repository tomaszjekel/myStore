using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Domain.Repositories
{
    public interface IFileRepository
    {
        Task<FilesUpload> GetAsync(Guid id);
        Task<IEnumerable<FilesUpload>> BrowseAsync(Guid userId);
        Task<IEnumerable<FilesUpload>> BrowseByProductAsync(Guid userId, Guid productId);
        Task CreateAsync(FilesUpload fu);
        Task UpdateAsync(Guid productId);
    }
}
