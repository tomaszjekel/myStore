using MyStore.Domain;
using MyStore.Services.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Services
{
    public interface IFileService
    {
        Task<FileDto> GetAsync(Guid id);
        Task<IEnumerable<FileDto>> BrowseAsync(Guid UserId);
        Task CreateAsync(Guid userId, Guid? productId, string name, DateTime date);
    }
}
