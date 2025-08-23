using System.Security.Claims;
using AgenciaViajes.API.Models;
using AgenciaViajes.API.Services;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgenciaViajes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly FirebaseService _fb;
    public FavoritesController(FirebaseService fb) => _fb = fb;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var ss = await _fb.Favorites.WhereEqualTo("UserId", UserId).GetSnapshotAsync();
        var list = ss.Documents.Select(d => d.ConvertTo<FavoriteCountry>()).OrderByDescending(f => f.AddedAt).ToList();
        return Ok(list);
    }

    [HttpPost("{countryCode}")]
    public async Task<IActionResult> Add(string countryCode)
    {
        // prevent duplicates
        var existing = await _fb.Favorites
            .WhereEqualTo("UserId", UserId)
            .WhereEqualTo("CountryCode", countryCode)
            .Limit(1).GetSnapshotAsync();
        if (existing.Documents.Count > 0) return Conflict("Ya es favorito.");

        var fav = new FavoriteCountry { UserId = UserId, CountryCode = countryCode };
        await _fb.Favorites.Document(fav.Id).SetAsync(fav);
        return CreatedAtAction(nameof(Get), new { id = fav.Id }, fav);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        var docRef = _fb.Favorites.Document(id);
        var snap = await docRef.GetSnapshotAsync();
        if (!snap.Exists) return NotFound();
        var fav = snap.ConvertTo<FavoriteCountry>();
        if (fav.UserId != UserId) return Forbid();
        await docRef.DeleteAsync();
        return NoContent();
    }
}