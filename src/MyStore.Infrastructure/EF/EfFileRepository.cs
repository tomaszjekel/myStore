using Microsoft.EntityFrameworkCore;
using MyStore.Domain;
using MyStore.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Infrastructure.EF
{
    public class EfFileRepository: IFileRepository
    {
        private readonly MyStoreContext _context;

        public EfFileRepository(MyStoreContext context)
        {
            _context = context;
        }

        public async Task<FilesUpload> GetAsync(Guid id)
            => await _context.Files.SingleOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<FilesUpload>> BrowseAsync(string name)
        {
            var files = _context.Files.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                files = files.Where(x => x.Name.Contains(name));
            }

            return await files.ToListAsync();
        }

        public async Task CreateAsync(FilesUpload files)
        {
            await _context.Files.AddAsync(files);
            await _context.SaveChangesAsync();
        }
    }
}
