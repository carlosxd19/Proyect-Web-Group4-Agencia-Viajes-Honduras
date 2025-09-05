using System.Collections.Generic;
using System.Threading.Tasks;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services
{
    public interface ICountryService
    {
        Task<IReadOnlyList<CountryBasic>> ListAsync(bool onlyActive);
        Task<CountryBasic?> GetAsync(string code);
        Task<CountryBasic> UpsertAsync(CountryBasic c); // code como id
    }
}
