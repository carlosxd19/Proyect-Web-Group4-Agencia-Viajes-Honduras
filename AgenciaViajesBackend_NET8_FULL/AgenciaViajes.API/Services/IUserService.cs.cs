using System.Threading.Tasks;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(User u);
        Task<User?> GetAsync(string id);
    }
}
