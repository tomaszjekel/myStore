using System;
using System.Threading.Tasks;

namespace MyStore.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetAsync(Guid id);
        Task<User> GetAsync(string email);
        //User GetUserId(string email);
        Task CreateAsync(User user);
        Task<string> Confirmation(string userId, string confirmationId);
        Task ResetPassword(User user, string password);
        Task<User> GetUserByResetPassword(string guid);
        Task UpdatePassword(string user, string password);
    }
}