using System;
using System.Threading.Tasks;

namespace MyStore.Framework
{
    public interface IAuthenticator
    {
        Task SignInAsync(Guid userId, string email, string role);
        Task SignOutAsync();
    }
}