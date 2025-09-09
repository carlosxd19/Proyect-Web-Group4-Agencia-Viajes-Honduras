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

        public async Task<IReadOnlyList<CountryBasic>> ListAsync(Region? region = null)
        {
            Query q = _db.Collection(COL);
            if (region.HasValue)
                q = q.WhereEqualTo("Region", region.Value.ToString());

            var snap = await q.GetSnapshotAsync();
            return snap.Documents
                       .Select(d => d.ConvertTo<CountryBasic>())
                       .OrderBy(c => c.Name)
                       .ToList();
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

        public async Task<int> UpsertManyAsync(IEnumerable<CountryBasic> countries)
        {
            var batch = _db.StartBatch();
            int count = 0;
            foreach (var c in countries)
            {
                c.Code = c.Code.ToUpper();
                var doc = _db.Collection(COL).Document(c.Code);
                batch.Set(doc, c, SetOptions.Overwrite);
                count++;
            }
            await batch.CommitAsync();
            return count;
        }

        public Task<object?> ListAsync(object region)
        {
            throw new NotImplementedException();
        }
    }
}
