using System.Collections.Generic;
using System.Threading.Tasks;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services
{
    public interface ITripService
    {
        Task<Trip> CreateAsync(Trip t);
        Task<Trip?> GetAsync(string id);
        Task<IReadOnlyList<Trip>> ListAsync();
        Task<IReadOnlyList<Trip>> ListByClientAsync(string clientId);
        Task<Trip?> UpdateAsync(Trip t);
        Task<bool> DeleteAsync(string id);
    }
}
