using System.Security.Claims;
using AgenciaViajes.API.DTOs;
using AgenciaViajes.API.Models;
using AgenciaViajes.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgenciaViajes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly FirebaseService _fb;

    public TripsController(FirebaseService fb) => _fb = fb;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!;

    [HttpGet]
    public async Task<IActionResult> GetMyTrips()
    {
        var ss = await _fb.Trips.WhereEqualTo("UserId", UserId).GetSnapshotAsync();
        var trips = ss.Documents.Select(d => d.ConvertTo<Trip>()).OrderBy(t => t.StartDate).ToList();
        return Ok(trips);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TripCreateRequest req)
    {
        var trip = new Trip
        {
            UserId = UserId,
            Title = req.Title,
            CountryCode = req.CountryCode,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            Status = req.Status,
            Description = req.Description
        };
        await _fb.Trips.Document(trip.Id).SetAsync(trip);
        return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var doc = await _fb.Trips.Document(id).GetSnapshotAsync();
        if (!doc.Exists) return NotFound();
        var t = doc.ConvertTo<Trip>();
        if (t.UserId != UserId) return Forbid();
        return Ok(t);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, TripUpdateRequest req)
    {
        var docRef = _fb.Trips.Document(id);
        var snap = await docRef.GetSnapshotAsync();
        if (!snap.Exists) return NotFound();
        var t = snap.ConvertTo<Trip>();
        if (t.UserId != UserId) return Forbid();

        t.Title = req.Title;
        t.CountryCode = req.CountryCode;
        t.StartDate = req.StartDate;
        t.EndDate = req.EndDate;
        t.Status = req.Status;
        t.Description = req.Description;

        await docRef.SetAsync(t);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var docRef = _fb.Trips.Document(id);
        var snap = await docRef.GetSnapshotAsync();
        if (!snap.Exists) return NotFound();
        var t = snap.ConvertTo<Trip>();
        if (t.UserId != UserId) return Forbid();
        await docRef.DeleteAsync();
        return NoContent();
    }
}
