using AgenciaViajes.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgenciaViajes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly CountryService _countries;
    public CountriesController(CountryService countries) => _countries = countries;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? name = null)
        => Ok(await _countries.GetAllAsync(name));

    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var c = await _countries.GetByCodeAsync(code);
        return c is null ? NotFound() : Ok(c);
    }
}