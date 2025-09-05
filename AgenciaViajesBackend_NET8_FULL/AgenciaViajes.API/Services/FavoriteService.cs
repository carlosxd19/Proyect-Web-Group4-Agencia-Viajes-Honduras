using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly FirestoreDb _db;
        private const string COL = "favoritos";
        public FavoriteService(FirestoreDb db) => _db = db;

        public async Task<IReadOnlyList<FavoriteCountry>> ListAsync(string clientId)
        {
            var snaps = await _db.Collection(COL)
                                 .WhereEqualTo("ClientId", clientId)
                                 .OrderByDescending("CreatedAt")
                                 .GetSnapshotAsync();
            return snaps.Documents.Select(d => d.ConvertTo<FavoriteCountry>()).ToList();
        }

        public async Task<FavoriteCountry> AddAsync(FavoriteCountry fav)
        {
            fav.CountryCode = fav.CountryCode.ToUpper();
            await _db.Collection(COL).Document(fav.Id).SetAsync(fav, SetOptions.Overwrite);
            return fav;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var doc = _db.Collection(COL).Document(id);
            var snap = await doc.GetSnapshotAsync();
            if (!snap.Exists) return false;
            await doc.DeleteAsync();
            return true;
        }
    }
}
