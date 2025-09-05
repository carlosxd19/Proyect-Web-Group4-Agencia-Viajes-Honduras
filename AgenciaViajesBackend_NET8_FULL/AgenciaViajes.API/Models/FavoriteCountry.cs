using Google.Cloud.Firestore;
using System;

namespace AgenciaViajes.API.Models
{
    [FirestoreData]
    public class FavoriteCountry
    {
        [FirestoreProperty] public string Id { get; set; } = Guid.NewGuid().ToString();
        [FirestoreProperty] public string ClientId { get; set; } = default!;
        [FirestoreProperty] public string CountryCode { get; set; } = default!;
        [FirestoreProperty] public string? City { get; set; }
        [FirestoreProperty] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
