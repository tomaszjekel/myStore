using System.Threading.Tasks;
using MyStore.Services.DTO;

namespace MyStore.Services
{
    public interface IUserService
    {
        Task<UserDto> GetAsync(string email);
        Task RegisterAsync(string email, string password, string role);
        Task LoginAsync(string email, string password);
    }
}