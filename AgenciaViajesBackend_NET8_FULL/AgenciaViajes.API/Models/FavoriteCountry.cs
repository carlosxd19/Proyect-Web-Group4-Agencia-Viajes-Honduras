using Google.Cloud.Firestore;

namespace AgenciaViajes.API.Models;

[FirestoreData]
public class Favorite
{
    [FirestoreDocumentId]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [FirestoreProperty]
    public string UserId { get; set; } = "";

    [FirestoreProperty]
    public string CountryCode { get; set; } = "";

    [FirestoreProperty]
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
