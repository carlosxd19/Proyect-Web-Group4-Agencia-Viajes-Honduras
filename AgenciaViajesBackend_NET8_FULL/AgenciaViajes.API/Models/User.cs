using Google.Cloud.Firestore;
using System;

namespace AgenciaViajes.API.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty] public string Id { get; set; } = Guid.NewGuid().ToString(); // idCliente
        [FirestoreProperty] public string FullName { get; set; } = default!;
        [FirestoreProperty] public string Email { get; set; } = default!;
        [FirestoreProperty] public string? PreferredCountryCode { get; set; }
        [FirestoreProperty] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
