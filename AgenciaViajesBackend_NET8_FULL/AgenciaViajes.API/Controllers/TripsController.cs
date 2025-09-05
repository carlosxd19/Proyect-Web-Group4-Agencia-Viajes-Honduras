using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AgenciaViajes.API.DTOs;
using AgenciaViajes.API.Models;
using AgenciaViajes.API.Services;

namespace AgenciaViajes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViajesController : ControllerBase
    {
        private readonly ITripService _svc;
        public ViajesController(ITripService svc) => _svc = svc;

        [HttpGet]
        [SwaggerOperation(Summary = "Lista viajes (filtro opcional por clientId)")]
        public async Task<ActionResult<IEnumerable<TripDto>>> List([FromQuery] string? clientId = null)
        {
            var list = string.IsNullOrWhiteSpace(clientId) ? await _svc.ListAsync() : await _svc.ListByClientAsync(clientId);
            return Ok(list.Select(Map).ToArray());
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Crear viaje (país, ciudad, estancia, tipo pasajero, fecha)")]
        public async Task<ActionResult<TripDto>> Create([FromBody] TripCreateRequest req)
        {
            var saved = await _svc.CreateAsync(new Trip
            {
                ClientId = req.ClientId,
                CountryCode = req.CountryCode.ToUpper(),
                City = req.City.Trim(),
                StayDays = req.StayDays,
                PassengerType = req.PassengerType,
                TravelDate = req.TravelDate
            });
            return CreatedAtAction(nameof(List), new { id = saved.Id }, Map(saved));
        }

        private static TripDto Map(Trip t) => new TripDto
        {
            Id = t.Id,
            ClientId = t.ClientId,
            CountryCode = t.CountryCode,
            City = t.City,
            StayDays = t.StayDays,
            PassengerType = t.PassengerType.ToString(),
            TravelDate = t.TravelDate,
            Status = t.Status.ToString(),
            CreatedAt = t.CreatedAt
        };
    }

    // /api/Trips/{id} GET/PUT/DELETE (como ya tienes)
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _svc;
        public TripsController(ITripService svc) => _svc = svc;

        [HttpGet("{id}")]
        public async Task<ActionResult<TripDto>> Get(string id)
        {
            var t = await _svc.GetAsync(id);
            return t is null ? NotFound() : Ok(new TripDto
            {
                Id = t.Id,
                ClientId = t.ClientId,
                CountryCode = t.CountryCode,
                City = t.City,
                StayDays = t.StayDays,
                PassengerType = t.PassengerType.ToString(),
                TravelDate = t.TravelDate,
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TripDto>> Update(string id, [FromBody] TripUpdateRequest req)
        {
            var updated = await _svc.UpdateAsync(new Trip
            {
                Id = id,
                CountryCode = req.CountryCode.ToUpper(),
                City = req.City.Trim(),
                StayDays = req.StayDays,
                PassengerType = req.PassengerType,
                TravelDate = req.TravelDate,
                Status = req.Status
            });
            return updated is null ? NotFound() : Ok(new TripDto
            {
                Id = updated.Id,
                ClientId = updated.ClientId,
                CountryCode = updated.CountryCode,
                City = updated.City,
                StayDays = updated.StayDays,
                PassengerType = updated.PassengerType.ToString(),
                TravelDate = updated.TravelDate,
                Status = updated.Status.ToString(),
                CreatedAt = updated.CreatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
            => (await _svc.DeleteAsync(id)) ? NoContent() : NotFound();
    }
}
