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
        Task CreateAsync(FilesUpload fu);
    }
}
