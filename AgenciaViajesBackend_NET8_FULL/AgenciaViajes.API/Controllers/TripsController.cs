using System.Security.Claims;
using AgenciaViajes.API.DTOs;
using AgenciaViajes.API.Models;
using AgenciaViajes.API.Services;
using Swashbuckle.AspNetCore.Annotations;
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

    /// <summary>
    /// Obtiene todos los viajes del usuario autenticado.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyTrips()
    {
        var trips = await _fb.QueryAsync<Trip>("trips", "UserId", UserId);
        return Ok(trips.OrderBy(t => t.StartDate));
    }

    /// <summary>
    /// Crea un nuevo viaje para el usuario autenticado.
    /// </summary>
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

        await _fb.AddAsync("trips", trip.Id, trip);
        return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
    }

    /// <summary>
    /// Obtiene un viaje por Id.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var trip = await _fb.GetAsync<Trip>("trips", id);
        if (trip == null) return NotFound();
        if (trip.UserId != UserId) return Forbid();
        return Ok(trip);
    }

    /// <summary>
    /// Actualiza un viaje por Id.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, TripUpdateRequest req)
    {
        var trip = await _fb.GetAsync<Trip>("trips", id);
        if (trip == null) return NotFound();
        if (trip.UserId != UserId) return Forbid();

        trip.Title = req.Title;
        trip.CountryCode = req.CountryCode;
        trip.StartDate = req.StartDate;
        trip.EndDate = req.EndDate;
        trip.Status = req.Status;
        trip.Description = req.Description;

        await _fb.UpdateAsync("trips", id, trip);
        return NoContent();
    }

    /// <summary>
    /// Elimina un viaje por Id.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var trip = await _fb.GetAsync<Trip>("trips", id);
        if (trip == null) return NotFound();
        if (trip.UserId != UserId) return Forbid();

        await _fb.DeleteAsync("trips", id);
        return NoContent();
    }
}
