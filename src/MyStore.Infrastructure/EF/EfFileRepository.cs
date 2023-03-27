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
    public class EfFileRepository : IFileRepository
    {
        private readonly MyStoreContext _context;

        public EfFileRepository(MyStoreContext context)
        {
            _context = context;
        }

        public async Task<FilesUpload> GetAsync(Guid id)
            => await _context.Files.SingleOrDefaultAsync(p => p.Id == id) ;

        public async Task<IEnumerable<FilesUpload>> BrowseAsync(Guid userId)
        {
            var files = _context.Files.AsQueryable();
            if (Guid.Empty != userId)
            {
                files = files.Where(x => x.UserId == userId&& x.ProductId==null);
            }

            return await files.ToListAsync();
        }

        public async Task<IEnumerable<FilesUpload>> BrowseByProductAsync(Guid userId, Guid productId)
        {
            var files = _context.Files.AsQueryable();
            if (Guid.Empty != userId)
            {
                files = files.Where(x => x.UserId == userId && x.ProductId == productId);
            }

            return await files.ToListAsync();
        }

        public async Task CreateAsync(FilesUpload files)
        {
            await _context.Files.AddAsync(files);
            try
            {
                await _context.SaveChangesAsync();
            }catch(Exception ex)
            {
                ;
            }
        }

        public async Task UpdateAsync(Guid productId)
        {
            //var u = _context.Files.Where(c => c.ProductId == null);
            //await u.ForEachAsync(b => b.ProductId = productId);
            ////await _context.AddAsync(t);
            //await _context.SaveChangesAsync();
        }
    }
}
