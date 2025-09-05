using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AgenciaViajes.API.Models;
using AgenciaViajes.API.Services;

namespace AgenciaViajes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _svc;
        public FavoritesController(IFavoriteService svc) => _svc = svc;

        [HttpGet]
        [SwaggerOperation(Summary = "Rutas guardadas por cliente")]
        public async Task<ActionResult<IEnumerable<FavoriteCountry>>> List([FromQuery] string clientId)
            => Ok(await _svc.ListAsync(clientId));

        [HttpPost("{countryCode}")]
        [SwaggerOperation(Summary = "Guardar ruta favorita (país y opcional ciudad)")]
        public async Task<ActionResult<FavoriteCountry>> Add(string countryCode, [FromQuery] string clientId, [FromQuery] string? city = null)
        {
            var saved = await _svc.AddAsync(new FavoriteCountry
            {
                ClientId = clientId,
                CountryCode = countryCode.ToUpper(),
                City = string.IsNullOrWhiteSpace(city) ? null : city
            });
            return Ok(saved);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Eliminar ruta favorita")]
        public async Task<IActionResult> Delete(string id)
            => (await _svc.RemoveAsync(id)) ? NoContent() : NotFound();
    }
}
