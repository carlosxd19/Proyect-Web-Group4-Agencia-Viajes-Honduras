using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services
{
    public class CountryService : ICountryService
    {
        private readonly FirestoreDb _db;
        private const string COL = "paises";
        public CountryService(FirestoreDb db) => _db = db;

        public async Task<IReadOnlyList<CountryBasic>> ListAsync(bool onlyActive)
        {
            Query q = _db.Collection(COL).OrderBy("Name");
            if (onlyActive) q = q.WhereEqualTo("IsActive", true);
            var snaps = await q.GetSnapshotAsync();
            return snaps.Documents.Select(d => d.ConvertTo<CountryBasic>()).ToList();
        }

        public async Task<CountryBasic?> GetAsync(string code)
        {
            var doc = await _db.Collection(COL).Document(code.ToUpper()).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<CountryBasic>() : null;
        }

        public async Task<CountryBasic> UpsertAsync(CountryBasic c)
        {
            c.Code = c.Code.ToUpper();
            await _db.Collection(COL).Document(c.Code).SetAsync(c, SetOptions.Overwrite);
            return c;
        }
    }
}
