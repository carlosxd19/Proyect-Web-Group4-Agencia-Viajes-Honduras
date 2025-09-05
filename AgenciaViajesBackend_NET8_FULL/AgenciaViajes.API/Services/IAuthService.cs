using System.Threading.Tasks;
using AgenciaViajes.API.DTOs;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest req);
        Task<User?> GetUserAsync(string clientId);
        Task<AuthResponse> LoginAsync(LoginRequest req); // demo/Swagger
    }
}
