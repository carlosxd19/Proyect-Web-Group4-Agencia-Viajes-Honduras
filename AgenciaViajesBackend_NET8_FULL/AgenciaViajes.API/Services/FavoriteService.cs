using AgenciaViajes.API.Models;
using Google.Cloud.Firestore;

namespace AgenciaViajes.API.Services;

public class FavoriteService
{
    private readonly FirestoreDb _db;
    public FavoriteService(FirestoreDb db) => _db = db;

    private CollectionReference FavCollection => _db.Collection("favorites");

    public async Task<List<Favorite>> GetByUser(string userId)
    {
        var snap = await FavCollection.WhereEqualTo("UserId", userId).GetSnapshotAsync();
        return snap.Documents.Select(d => d.ConvertTo<Favorite>()).ToList();
    }

    public async Task<Favorite> Add(string userId, string countryCode)
    {
        var fav = new Favorite { UserId = userId, CountryCode = countryCode };
        await FavCollection.Document(fav.Id).SetAsync(fav);
        return fav;
    }

    public async Task Remove(string userId, string id)
    {
        var doc = FavCollection.Document(id);
        var snap = await doc.GetSnapshotAsync();
        if (snap.Exists)
        {
            var fav = snap.ConvertTo<Favorite>();
            if (fav.UserId == userId)
                await doc.DeleteAsync();
        }
    }
}
