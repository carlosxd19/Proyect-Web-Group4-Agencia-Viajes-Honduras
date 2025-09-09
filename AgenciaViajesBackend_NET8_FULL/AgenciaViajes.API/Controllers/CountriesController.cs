using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AgenciaViajes.API.DTOs;
using AgenciaViajes.API.Models;
using AgenciaViajes.API.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgenciaViajes.API.Controllers
{
    [ApiController]
    [Route("api/Países")] // <- sección única en Swagger
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _svc;
        public CountriesController(ICountryService svc) => _svc = svc;

        // GET /api/Países?region=Europa -> lista desde Firestore (para llenar combos)
        [HttpGet]
        [SwaggerOperation(Summary = "Lista países (opcional: filtra por región)")]
        public async Task<ActionResult<IEnumerable<CountryBasic>>> GetAll([FromQuery] Region? region = null)
        {
            var result = await _svc.ListAsync(region);
            return Ok(result);
        }

        // GET /api/Países/{code} -> obtiene un país por código (documento)
        [HttpGet("{code}")]
        public async Task<ActionResult<CountryBasic>> GetByCode(string code)
        {
            var c = await _svc.GetAsync(code.ToUpper());
            return c is null ? NotFound() : Ok(c);
        }

        // POST /api/Países -> crear/actualizar un país manualmente (cuerpo CountryBasic)
        [HttpPost]
        [SwaggerOperation(Summary = "Crear/actualizar un país (manual)")]
        public async Task<ActionResult<CountryBasic>> Upsert([FromBody] CountryBasic c)
            => Ok(await _svc.UpsertAsync(c));

        // POST /api/Países/seleccionar -> guardar país elegido desde un selector (enum)
        // Útil cuando quieres que el usuario elija “a qué país de Europa quiere ir”
        [HttpPost("seleccionar")]
        [SwaggerOperation(Summary = "Guardar un país eligiéndolo desde un selector (Europa)")]
        public async Task<ActionResult<CountryBasic>> CreateFromSelection([FromBody] CountrySelectionRequest req)
        {
            var (code, name) = MapEurope(req.Country);
            if (string.IsNullOrEmpty(code)) return BadRequest("País inválido.");

            var country = new CountryBasic
            {
                Code = code,     // p.ej. "ALEMANIA" (como en tu Firebase)
                Name = name,     // p.ej. "Alemania"
                IsActive = req.IsActive,
                Region = Region.Europa,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _svc.UpsertAsync(country);
            return Ok(saved);
        }

        // --- Listas predefinidas (no BD) por si quieres pintar combos sin tocar Firestore ---
        // GET /api/Países/filtrar
        [HttpGet("filtrar")]
        [SwaggerOperation(Summary = "Lista países por región (predefinido)")]
        public ActionResult<IEnumerable<object>> ListByRegion([FromQuery] Region region = Region.Europa)
        {
            if (region == Region.Europa)
            {
                var europe = new[]
                {
                    new { code = "ESPAÑA",       name = "España" },
                    new { code = "FRANCIA",      name = "Francia" },
                    new { code = "ALEMANIA",     name = "Alemania" },
                    new { code = "ITALIA",       name = "Italia" },
                    new { code = "PORTUGAL",     name = "Portugal" },
                    new { code = "REINO UNIDO",  name = "Reino Unido" },
                    new { code = "PAISES BAJOS", name = "Países Bajos" },
                    new { code = "BELGICA",      name = "Bélgica" },
                    new { code = "SUIZA",        name = "Suiza" },
                    new { code = "SUECIA",       name = "Suecia" }
                };
                return Ok(europe);
            }
            return Ok(Array.Empty<object>());
        }

        // GET /api/Países/europa -> atajo de la lista predefinida
        [HttpGet("europa")]
        [SwaggerOperation(Summary = "Lista de 10 países de Europa (predefinido)")]
        public ActionResult<IEnumerable<object>> EuropeList()
            => ListByRegion(Region.Europa);

        // POST /api/Países/semilla/europa -> siembra los 10 en Firestore
        [HttpPost("semilla/europa")]
        [SwaggerOperation(Summary = "Guardar en BD los 10 países de Europa")]
        public async Task<ActionResult<object>> SeedEurope()
        {
            var europe = new[]
            {
                new CountryBasic { Code = "ESPAÑA",       Name = "España",       IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "FRANCIA",      Name = "Francia",      IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "ALEMANIA",     Name = "Alemania",     IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "ITALIA",       Name = "Italia",       IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "PORTUGAL",     Name = "Portugal",     IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "REINO UNIDO",  Name = "Reino Unido",  IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "PAISES BAJOS", Name = "Países Bajos", IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "BELGICA",      Name = "Bélgica",      IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "SUIZA",        Name = "Suiza",        IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "SUECIA",       Name = "Suecia",       IsActive = true, Region = Region.Europa }
            };
            var count = await _svc.UpsertManyAsync(europe);
            return Ok(new { insertedOrUpdated = count });
        }

        // --- utilidades privadas ---
        private static (string Code, string Name) MapEurope(CountryEurope c) => c switch
        {
            CountryEurope.España => ("ESPAÑA", "España"),
            CountryEurope.Francia => ("FRANCIA", "Francia"),
            CountryEurope.Alemania => ("ALEMANIA", "Alemania"),
            CountryEurope.Italia => ("ITALIA", "Italia"),
            CountryEurope.Portugal => ("PORTUGAL", "Portugal"),
            CountryEurope.ReinoUnido => ("REINO UNIDO", "Reino Unido"),
            CountryEurope.PaisesBajos => ("PAISES BAJOS", "Países Bajos"),
            CountryEurope.Belgica => ("BELGICA", "Bélgica"),
            CountryEurope.Suiza => ("SUIZA", "Suiza"),
            CountryEurope.Suecia => ("SUECIA", "Suecia"),
            _ => ("", "")
        };
    }
}
