using System.Security.Claims;
using AgenciaViajes.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgenciaViajes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly FirebaseUserService _firebaseUserService;

    public UsersController(FirebaseUserService firebaseUserService)
    {
        _firebaseUserService = firebaseUserService;
    }

    // Registro de usuario en Firebase
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(string email, string password, string name)
    {
        var user = await _firebaseUserService.CreateUserAsync(email, password, name);
        return Ok(new { user.Uid, user.Email, user.DisplayName });
    }

    // Perfil del usuario autenticado
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null) return Unauthorized();

        var u = await _firebaseUserService.GetUserByEmail(email);
        if (u is null) return NotFound();

        return Ok(new { u.Uid, u.Email, u.DisplayName });
    }
}
