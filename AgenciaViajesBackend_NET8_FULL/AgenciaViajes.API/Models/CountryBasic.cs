using Google.Cloud.Firestore;
using System;

namespace AgenciaViajes.API.Models
{
    [FirestoreData]
    public class CountryBasic
    {
        [FirestoreProperty] public string Code { get; set; } = default!; // ES, US, HN...
        [FirestoreProperty] public string Name { get; set; } = default!;
        [FirestoreProperty] public bool IsActive { get; set; } = true;
        [FirestoreProperty] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
