using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AgenciaViajes.API.DTOs;
using AgenciaViajes.API.Models;
using AgenciaViajes.API.Services;

namespace AgenciaViajes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _users;
        public AuthController(IUserService users) => _users = users;

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Registrar cliente", Description = "Guarda cliente con idCliente, nombre, correo y país destino preferido")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
        {
            var u = new User
            {
                Id = string.IsNullOrWhiteSpace(req.ClientId) ? Guid.NewGuid().ToString() : req.ClientId,
                FullName = req.FullName.Trim(),
                Email = req.Email.Trim().ToLower(),
                PreferredCountryCode = req.DestinationCountryCode
            };
            var saved = await _users.RegisterAsync(u);
            return Ok(new AuthResponse
            {
                ClientId = saved.Id,
                FullName = saved.FullName,
                Email = saved.Email,
                DestinationCountryCode = saved.PreferredCountryCode
            });
        }

        [HttpPost("login")]
        public ActionResult<AuthResponse> Login([FromBody] LoginRequest req)
        {
            // Este endpoint es solo eco para tu front: puedes validarlo en Firestore si quieres
            return Ok(new AuthResponse
            {
                ClientId = req.ClientId,
                FullName = "Cliente",
                Email = req.Email
            });
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> Me([FromQuery] string clientId)
        {
            var u = await _users.GetAsync(clientId);
            if (u is null) return NotFound();
            return Ok(new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PreferredCountryCode = u.PreferredCountryCode,
                CreatedAt = u.CreatedAt
            });
        }
    }
}
