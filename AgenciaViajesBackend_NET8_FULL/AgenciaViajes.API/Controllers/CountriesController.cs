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
    [Route("api/Pa�ses")] // <- secci�n �nica en Swagger
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _svc;
        public CountriesController(ICountryService svc) => _svc = svc;

        // GET /api/Pa�ses?region=Europa -> lista desde Firestore (para llenar combos)
        [HttpGet]
        [SwaggerOperation(Summary = "Lista pa�ses (opcional: filtra por regi�n)")]
        public async Task<ActionResult<IEnumerable<CountryBasic>>> GetAll([FromQuery] Region? region = null)
        {
            var result = await _svc.ListAsync(region);
            return Ok(result);
        }

        // GET /api/Pa�ses/{code} -> obtiene un pa�s por c�digo (documento)
        [HttpGet("{code}")]
        public async Task<ActionResult<CountryBasic>> GetByCode(string code)
        {
            var c = await _svc.GetAsync(code.ToUpper());
            return c is null ? NotFound() : Ok(c);
        }

        // POST /api/Pa�ses -> crear/actualizar un pa�s manualmente (cuerpo CountryBasic)
        [HttpPost]
        [SwaggerOperation(Summary = "Crear/actualizar un pa�s (manual)")]
        public async Task<ActionResult<CountryBasic>> Upsert([FromBody] CountryBasic c)
            => Ok(await _svc.UpsertAsync(c));

        // POST /api/Pa�ses/seleccionar -> guardar pa�s elegido desde un selector (enum)
        // �til cuando quieres que el usuario elija �a qu� pa�s de Europa quiere ir�
        [HttpPost("seleccionar")]
        [SwaggerOperation(Summary = "Guardar un pa�s eligi�ndolo desde un selector (Europa)")]
        public async Task<ActionResult<CountryBasic>> CreateFromSelection([FromBody] CountrySelectionRequest req)
        {
            var (code, name) = MapEurope(req.Country);
            if (string.IsNullOrEmpty(code)) return BadRequest("Pa�s inv�lido.");

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
        // GET /api/Pa�ses/filtrar
        [HttpGet("filtrar")]
        [SwaggerOperation(Summary = "Lista pa�ses por regi�n (predefinido)")]
        public ActionResult<IEnumerable<object>> ListByRegion([FromQuery] Region region = Region.Europa)
        {
            if (region == Region.Europa)
            {
                var europe = new[]
                {
                    new { code = "ESPA�A",       name = "Espa�a" },
                    new { code = "FRANCIA",      name = "Francia" },
                    new { code = "ALEMANIA",     name = "Alemania" },
                    new { code = "ITALIA",       name = "Italia" },
                    new { code = "PORTUGAL",     name = "Portugal" },
                    new { code = "REINO UNIDO",  name = "Reino Unido" },
                    new { code = "PAISES BAJOS", name = "Pa�ses Bajos" },
                    new { code = "BELGICA",      name = "B�lgica" },
                    new { code = "SUIZA",        name = "Suiza" },
                    new { code = "SUECIA",       name = "Suecia" }
                };
                return Ok(europe);
            }
            return Ok(Array.Empty<object>());
        }

        // GET /api/Pa�ses/europa -> atajo de la lista predefinida
        [HttpGet("europa")]
        [SwaggerOperation(Summary = "Lista de 10 pa�ses de Europa (predefinido)")]
        public ActionResult<IEnumerable<object>> EuropeList()
            => ListByRegion(Region.Europa);

        // POST /api/Pa�ses/semilla/europa -> siembra los 10 en Firestore
        [HttpPost("semilla/europa")]
        [SwaggerOperation(Summary = "Guardar en BD los 10 pa�ses de Europa")]
        public async Task<ActionResult<object>> SeedEurope()
        {
            var europe = new[]
            {
                new CountryBasic { Code = "ESPA�A",       Name = "Espa�a",       IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "FRANCIA",      Name = "Francia",      IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "ALEMANIA",     Name = "Alemania",     IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "ITALIA",       Name = "Italia",       IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "PORTUGAL",     Name = "Portugal",     IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "REINO UNIDO",  Name = "Reino Unido",  IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "PAISES BAJOS", Name = "Pa�ses Bajos", IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "BELGICA",      Name = "B�lgica",      IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "SUIZA",        Name = "Suiza",        IsActive = true, Region = Region.Europa },
                new CountryBasic { Code = "SUECIA",       Name = "Suecia",       IsActive = true, Region = Region.Europa }
            };
            var count = await _svc.UpsertManyAsync(europe);
            return Ok(new { insertedOrUpdated = count });
        }

        // --- utilidades privadas ---
        private static (string Code, string Name) MapEurope(CountryEurope c) => c switch
        {
            CountryEurope.Espa�a => ("ESPA�A", "Espa�a"),
            CountryEurope.Francia => ("FRANCIA", "Francia"),
            CountryEurope.Alemania => ("ALEMANIA", "Alemania"),
            CountryEurope.Italia => ("ITALIA", "Italia"),
            CountryEurope.Portugal => ("PORTUGAL", "Portugal"),
            CountryEurope.ReinoUnido => ("REINO UNIDO", "Reino Unido"),
            CountryEurope.PaisesBajos => ("PAISES BAJOS", "Pa�ses Bajos"),
            CountryEurope.Belgica => ("BELGICA", "B�lgica"),
            CountryEurope.Suiza => ("SUIZA", "Suiza"),
            CountryEurope.Suecia => ("SUECIA", "Suecia"),
            _ => ("", "")
        };
    }
}
