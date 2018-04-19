using System.Threading.Tasks;

namespace MyStore.Framework
{
    public interface IAuthenticator
    {
        Task SignInAsync(string email, string role);
        Task SignOutAsync();
    }
}