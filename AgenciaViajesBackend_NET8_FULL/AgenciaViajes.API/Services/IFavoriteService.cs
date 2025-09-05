using System.Collections.Generic;
using System.Threading.Tasks;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services
{
    public interface IFavoriteService
    {
        Task<IReadOnlyList<FavoriteCountry>> ListAsync(string clientId);
        Task<FavoriteCountry> AddAsync(FavoriteCountry fav);
        Task<bool> RemoveAsync(string id);
    }
}
