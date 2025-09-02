using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;

namespace AgenciaViajes.API.Services
{
    public class FirebaseUserService
    {
        private readonly FirestoreDb _db;

        public FirebaseUserService()
        {
            _db = FirestoreDb.Create("interviajes-af2ed"); // tu ProjectId
        }

        public async Task<UserRecord> CreateUserAsync(string email, string password, string name)
        {
            var args = new UserRecordArgs()
            {
                Email = email,
                EmailVerified = false,
                Password = password,
                DisplayName = name,
                Disabled = false,
            };

            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

            // Guardar en Firestore también
            var userRef = _db.Collection("usuarios").Document(userRecord.Uid);
            await userRef.SetAsync(new
            {
                Email = email,
                Name = name,
                RegisteredAt = DateTime.UtcNow
            });

            return userRecord;
        }

        public async Task<UserRecord?> GetUserByEmail(string email)
        {
            try
            {
                return await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
            }
            catch
            {
                return null;
            }
        }
    }
}
