using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AgenciaViajes.API.DTOs;
using AgenciaViajes.API.Models;
using AgenciaViajes.API.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AgenciaViajes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViajesController : ControllerBase
    {
        private readonly ITripService _svc;
        private readonly ICountryService _countrySvc;

        public ViajesController(ITripService svc, ICountryService countrySvc)
        {
            _svc = svc;
            _countrySvc = countrySvc;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista de viajes")]
        public async Task<ActionResult<IEnumerable<TripDto>>> List()
        {
            var items = await _svc.ListAsync();
            return Ok(items.Select(t => new TripDto
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
            }));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtiene un viaje por id")]
        public async Task<ActionResult<TripDto>> GetById(string id)
        {
            var t = await _svc.GetAsync(id);
            if (t is null) return NotFound();

            return Ok(new TripDto
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

        [HttpPost]
        [SwaggerOperation(Summary = "Crear viaje")]
        public async Task<ActionResult<TripDto>> Create([FromBody] TripCreateRequest req)
        {
            // Validación de país
            var code = req.CountryCode?.Trim().ToUpper();
            var country = await _countrySvc.GetAsync(code!);

            if (country is null)
                return BadRequest($"El país '{code}' no existe en el catálogo.");
            if (!country.IsActive)
                return BadRequest($"El país '{code}' está inactivo y no puede seleccionarse.");

            // Si quieres exigir que sea SOLO Europa, descomenta:
            // if (country.Region != Region.Europa)
            //     return BadRequest("Solo se permiten países de Europa para este tipo de viaje.");

            var t = new Trip
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = req.ClientId,
                CountryCode = code!,
                City = req.City,
                StayDays = req.StayDays,
                PassengerType = req.PassengerType,
                TravelDate = req.TravelDate,
                Status = TripStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _svc.CreateAsync(t);

            var dto = new TripDto
            {
                Id = created.Id,
                ClientId = created.ClientId,
                CountryCode = created.CountryCode,
                City = created.City,
                StayDays = created.StayDays,
                PassengerType = created.PassengerType.ToString(),
                TravelDate = created.TravelDate,
                Status = created.Status.ToString(),
                CreatedAt = created.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TripDto>> Update(string id, [FromBody] TripUpdateRequest req)
        {
            // (tu lógica actual; si quieres, agrega misma validación de país si permite cambiar CountryCode)
            // ...
            return NotFound(); // placeholder si aún no lo implementas aquí
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
            => (await _svc.DeleteAsync(id)) ? NoContent() : NotFound();
    }
}
