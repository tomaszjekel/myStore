using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStore.Domain;
using MyStore.Domain.Repositories;

namespace MyStore.Infrastructure.EF
{
    public class EfUserRepository : IUserRepository
    {
        private readonly MyStoreContext _context;

        public EfUserRepository(MyStoreContext context)
        {
            _context = context;
        }

        public async Task<User> GetAsync(Guid id)
            => await _context.Users
                .SingleOrDefaultAsync(u => u.Id == id);

        public async Task<User> GetAsync(string email)
            => await _context.Users
                .SingleOrDefaultAsync(u => u.Email == email);

        public async Task CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}