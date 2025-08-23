using System.Security.Claims;
using AgenciaViajes.API.DTOs;
using AgenciaViajes.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgenciaViajes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
    {
        var user = await _auth.RegisterAsync(req.Email, req.Password, req.Name);
        var token = _auth.CreateJwt(user);
        return Ok(new AuthResponse(token, user.Id, user.Email, user.Name));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        var user = await _auth.ValidateCredentialsAsync(req.Email, req.Password);
        var token = _auth.CreateJwt(user);
        return Ok(new AuthResponse(token, user.Id, user.Email, user.Name));
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var email = User.FindFirstValue(ClaimTypes.Email);
        var name = User.FindFirstValue("name");
        return Ok(new { userId = uid, email, name });
    }
}