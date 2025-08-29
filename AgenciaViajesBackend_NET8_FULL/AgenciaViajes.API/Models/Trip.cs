using Google.Cloud.Firestore;

namespace AgenciaViajes.API.Models;

[FirestoreData]
public class Trip
{
    [FirestoreDocumentId]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [FirestoreProperty]
    public string UserId { get; set; } = "";

    [FirestoreProperty]
    public string Title { get; set; } = "";

    [FirestoreProperty]
    public string CountryCode { get; set; } = "";

    [FirestoreProperty]
    public DateTime StartDate { get; set; }

    [FirestoreProperty]
    public DateTime EndDate { get; set; }

    [FirestoreProperty]
    public string Status { get; set; } = "Planned";

    [FirestoreProperty]
    public string? Description { get; set; }
}
