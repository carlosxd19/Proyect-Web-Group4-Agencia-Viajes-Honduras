using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AgenciaViajes.API.Models;
using AgenciaViajes.API.Services;

namespace AgenciaViajes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _svc;
        public CountriesController(ICountryService svc) => _svc = svc;

        [HttpGet]
        [SwaggerOperation(Summary = "Lista países (para cuadro de selección)")]
        public async Task<ActionResult<IEnumerable<CountryBasic>>> GetAll([FromQuery] bool onlyActive = true)
            => Ok(await _svc.ListAsync(onlyActive));

        [HttpGet("{code}")]
        public async Task<ActionResult<CountryBasic>> GetByCode(string code)
        {
            var c = await _svc.GetAsync(code.ToUpper());
            return c is null ? NotFound() : Ok(c);
        }

        [HttpPost] // útil para cargar/actualizar catálogo
        public async Task<ActionResult<CountryBasic>> Upsert([FromBody] CountryBasic c)
            => Ok(await _svc.UpsertAsync(c));
    }

    // Alias /api/Paises
    [ApiController]
    [Route("api/[controller]")]
    public class PaisesController : CountriesController
    {
        public PaisesController(ICountryService svc) : base(svc) { }
    }
}
