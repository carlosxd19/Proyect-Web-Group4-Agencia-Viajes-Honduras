using System;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using AgenciaViajes.API.DTOs;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly FirestoreDb _db;
        private const string COL = "usuarios";

        public AuthService(FirestoreDb db) => _db = db;

        public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
        {
            var clientId = string.IsNullOrWhiteSpace(req.ClientId)
                ? Guid.NewGuid().ToString()
                : req.ClientId.Trim();

            var u = new User
            {
                Id = clientId,
                FullName = req.FullName.Trim(),
                Email = req.Email.Trim().ToLowerInvariant(),
                PreferredCountryCode = string.IsNullOrWhiteSpace(req.DestinationCountryCode)
                    ? null : req.DestinationCountryCode.Trim().ToUpperInvariant(),
                CreatedAt = DateTime.UtcNow
            };

            await _db.Collection(COL).Document(u.Id).SetAsync(u, SetOptions.Overwrite);

            return new AuthResponse
            {
                ClientId = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                DestinationCountryCode = u.PreferredCountryCode
            };
        }

        public async Task<User?> GetUserAsync(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId)) return null;
            var snap = await _db.Collection(COL).Document(clientId).GetSnapshotAsync();
            return snap.Exists ? snap.ConvertTo<User>() : null;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest req)
        {
            var u = await GetUserAsync(req.ClientId);
            if (u != null)
            {
                return new AuthResponse
                {
                    ClientId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    DestinationCountryCode = u.PreferredCountryCode
                };
            }

            // Eco para demo si no existe
            return new AuthResponse { ClientId = req.ClientId, FullName = "Cliente", Email = req.Email };
        }
    }
}
