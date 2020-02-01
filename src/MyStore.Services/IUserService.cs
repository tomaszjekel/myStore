using System.Threading.Tasks;
using MyStore.Services.DTO;

namespace MyStore.Services
{
    public interface IUserService
    {
        Task<UserDto> GetAsync(string email);
        Task RegisterAsync(string email, string password, string role);
        Task<bool> LoginAsync(string email, string password);
        Task<string> Confirmation(string userId, string confirmationId);
        Task<bool> ResetPassword(string email);
        Task<bool> RegisterNewPassword(string password, string guid);
    }
}