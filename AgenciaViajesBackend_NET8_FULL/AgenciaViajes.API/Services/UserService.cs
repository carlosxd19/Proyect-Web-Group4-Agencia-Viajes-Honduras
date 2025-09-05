using System.Threading.Tasks;
using Google.Cloud.Firestore;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services
{
    public class UserService : IUserService
    {
        private readonly FirestoreDb _db;
        private const string COL = "usuarios";
        public UserService(FirestoreDb db) => _db = db;

        public async Task<User> RegisterAsync(User u)
        {
            await _db.Collection(COL).Document(u.Id).SetAsync(u, SetOptions.Overwrite);
            return u;
        }

        public async Task<User?> GetAsync(string id)
        {
            var snap = await _db.Collection(COL).Document(id).GetSnapshotAsync();
            return snap.Exists ? snap.ConvertTo<User>() : null;
        }
    }
}
