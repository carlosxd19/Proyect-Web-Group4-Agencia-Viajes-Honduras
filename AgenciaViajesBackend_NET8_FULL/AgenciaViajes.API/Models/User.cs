using Google.Cloud.Firestore;

namespace AgenciaViajes.API.Models;

[FirestoreData]
public class User
{
    [FirestoreDocumentId]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [FirestoreProperty]
    public string Email { get; set; } = "";

    [FirestoreProperty]
    public string Name { get; set; } = "";

    [FirestoreProperty]
    public string PasswordHash { get; set; } = "";

    [FirestoreProperty]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
    