using System.Security.Claims;
using AgenciaViajes.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgenciaViajes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AuthService _auth;
    public UsersController(AuthService auth) => _auth = auth;

    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email)!;
        var u = await _auth.GetUserByEmail(email);
        if (u is null) return NotFound();
        return Ok(new { u.Id, u.Email, u.Name, u.CreatedAt });

    }
}