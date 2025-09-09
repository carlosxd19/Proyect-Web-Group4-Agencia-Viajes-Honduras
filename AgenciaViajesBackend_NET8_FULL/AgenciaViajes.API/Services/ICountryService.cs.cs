using System.Collections.Generic;
using System.Threading.Tasks;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services
{
    public interface ICountryService
    {
        Task<IReadOnlyList<CountryBasic>> ListAsync(Region? region = null);
        Task<CountryBasic?> GetAsync(string code);
        Task<CountryBasic> UpsertAsync(CountryBasic c);
        Task<int> UpsertManyAsync(IEnumerable<CountryBasic> countries);
        Task<object?> ListAsync(object region);
    }
}
