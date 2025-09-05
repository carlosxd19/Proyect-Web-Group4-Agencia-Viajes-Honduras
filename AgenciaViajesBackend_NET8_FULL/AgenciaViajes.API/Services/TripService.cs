using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services
{
    public class TripService : ITripService
    {
        private readonly FirestoreDb _db;
        private const string COL = "viajes";
        public TripService(FirestoreDb db) => _db = db;

        public async Task<Trip> CreateAsync(Trip t)
        {
            t.CountryCode = t.CountryCode.ToUpper();
            await _db.Collection(COL).Document(t.Id).SetAsync(t, SetOptions.Overwrite);
            return t;
        }

        public async Task<Trip?> GetAsync(string id)
        {
            var snap = await _db.Collection(COL).Document(id).GetSnapshotAsync();
            return snap.Exists ? snap.ConvertTo<Trip>() : null;
        }

        public async Task<IReadOnlyList<Trip>> ListAsync()
        {
            var snaps = await _db.Collection(COL).OrderByDescending("CreatedAt").GetSnapshotAsync();
            return snaps.Documents.Select(d => d.ConvertTo<Trip>()).ToList();
        }

        public async Task<IReadOnlyList<Trip>> ListByClientAsync(string clientId)
        {
            var snaps = await _db.Collection(COL)
                                 .WhereEqualTo("ClientId", clientId)
                                 .OrderByDescending("CreatedAt")
                                 .GetSnapshotAsync();
            return snaps.Documents.Select(d => d.ConvertTo<Trip>()).ToList();
        }

        public async Task<Trip?> UpdateAsync(Trip t)
        {
            var doc = _db.Collection(COL).Document(t.Id);
            var snap = await doc.GetSnapshotAsync();
            if (!snap.Exists) return null;
            t.CountryCode = t.CountryCode.ToUpper();
            await doc.SetAsync(t, SetOptions.Overwrite);
            return t;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var doc = _db.Collection(COL).Document(id);
            var snap = await doc.GetSnapshotAsync();
            if (!snap.Exists) return false;
            await doc.DeleteAsync();
            return true;
        }
    }
}
