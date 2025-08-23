using Google.Cloud.Firestore;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services;

public class FirebaseService
{
    private readonly FirestoreDb _db;
    public FirebaseService(FirestoreDb db) => _db = db;

    public CollectionReference Users => _db.Collection("users");
    public CollectionReference Trips => _db.Collection("trips");
    public CollectionReference Favorites => _db.Collection("favorites");
}