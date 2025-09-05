using Microsoft.AspNetCore.Mvc;
using AgenciaViajes.API.DTOs;
using AgenciaViajes.API.Models;
using AgenciaViajes.API.Services;

namespace AgenciaViajes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUserService _users;
        private readonly ITripService _trips;
        public UsuariosController(IUserService users, ITripService trips)
        { _users = users; _trips = trips; }

        [HttpPost("registrarse")]
        public async Task<ActionResult<UserDto>> RegisterUser([FromBody] RegisterRequest req)
        {
            var saved = await _users.RegisterAsync(new User
            {
                Id = string.IsNullOrWhiteSpace(req.ClientId) ? Guid.NewGuid().ToString() : req.ClientId,
                FullName = req.FullName.Trim(),
                Email = req.Email.Trim().ToLower(),
                PreferredCountryCode = req.DestinationCountryCode
            });
            return Ok(new UserDto
            {
                Id = saved.Id,
                FullName = saved.FullName,
                Email = saved.Email,
                PreferredCountryCode = saved.PreferredCountryCode,
                CreatedAt = saved.CreatedAt
            });
        }

        [HttpGet("perfil")]
        public async Task<ActionResult<UserDto>> Profile([FromQuery] string clientId)
        {
            var u = await _users.GetAsync(clientId);
            return u is null ? NotFound() : Ok(new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PreferredCountryCode = u.PreferredCountryCode,
                CreatedAt = u.CreatedAt
            });
        }

        [HttpGet("{clientId}/viajes")]
        public async Task<ActionResult<IEnumerable<TripDto>>> ClientTrips(string clientId)
            => Ok((await _trips.ListByClientAsync(clientId)).Select(t => new TripDto
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
}
