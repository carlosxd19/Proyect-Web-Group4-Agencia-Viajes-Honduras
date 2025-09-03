using Google.Cloud.Firestore;

namespace AgenciaViajes.API.DTOs;

[FirestoreData]
public class UserDto
{
    [FirestoreProperty]
    public string Id { get; set; } = default!;

    [FirestoreProperty]
    public string Email { get; set; } = default!;

    [FirestoreProperty]
    public string Name { get; set; } = default!;

    [FirestoreProperty]
    public string Password { get; set; } = default!; // Hash con BCrypt

    [FirestoreProperty]
    public DateTime RegisteredAt { get; set; }
}
