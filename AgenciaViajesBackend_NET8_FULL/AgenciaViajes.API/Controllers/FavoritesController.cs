using System.Security.Claims;
using AgenciaViajes.API.Models;
using AgenciaViajes.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgenciaViajes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly FavoriteService _service;
    public FavoritesController(FavoriteService service) => _service = service;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var favs = await _service.GetByUser(UserId);
        return Ok(favs);
    }

    [HttpPost("{countryCode}")]
    public async Task<IActionResult> Add(string countryCode)
    {
        var fav = await _service.Add(UserId, countryCode);
        return Ok(fav);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _service.Remove(UserId, id);
        return NoContent();
    }
}
