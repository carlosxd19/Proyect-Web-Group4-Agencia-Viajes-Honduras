using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services;

public class FirebaseService
{
    private readonly FirestoreDb _db;
    private readonly ILogger<FirebaseService> _logger;

    public FirebaseService(FirestoreDb db, ILogger<FirebaseService> logger)
    {
        _db = db;
        _logger = logger;
    }

    // 🔹 Referencias a colecciones
    public CollectionReference Users => _db.Collection("users");
    public CollectionReference Trips => _db.Collection("trips");
    public CollectionReference Favorites => _db.Collection("favorites");

    // 🔹 Agregar documento genérico
    public async Task AddAsync<T>(string collection, string id, T data)
    {
        try
        {
            await _db.Collection(collection).Document(id).SetAsync(data);
            _logger.LogInformation("Se agregó {Id} en {Collection}", id, collection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar {Id} en {Collection}", id, collection);
            throw;
        }
    }

    // 🔹 Leer documento
    public async Task<T?> GetAsync<T>(string collection, string id)
    {
        try
        {
            var doc = await _db.Collection(collection).Document(id).GetSnapshotAsync();
            if (!doc.Exists) return default;
            return doc.ConvertTo<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer {Id} de {Collection}", id, collection);
            throw;
        }
    }

    // 🔹 Actualizar documento
    public async Task UpdateAsync<T>(string collection, string id, T data)
    {
        try
        {
            await _db.Collection(collection).Document(id).SetAsync(data, SetOptions.Overwrite);
            _logger.LogInformation("Se actualizó {Id} en {Collection}", id, collection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar {Id} en {Collection}", id, collection);
            throw;
        }
    }

    // 🔹 Eliminar documento
    public async Task DeleteAsync(string collection, string id)
    {
        try
        {
            await _db.Collection(collection).Document(id).DeleteAsync();
            _logger.LogInformation("Se eliminó {Id} de {Collection}", id, collection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar {Id} de {Collection}", id, collection);
            throw;
        }
    }

    // 🔹 Consultar lista con filtro
    public async Task<List<T>> QueryAsync<T>(string collection, string field, object value)
    {
        try
        {
            var snapshot = await _db.Collection(collection).WhereEqualTo(field, value).GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<T>()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar {Collection} filtrando {Field}={Value}", collection, field, value);
            throw;
        }
    }
}
