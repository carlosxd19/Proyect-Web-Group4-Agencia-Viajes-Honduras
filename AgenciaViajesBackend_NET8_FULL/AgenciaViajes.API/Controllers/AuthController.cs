using System.Security.Claims;
using AgenciaViajes.API.DTOs;
using AgenciaViajes.API.Services;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgenciaViajes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    private readonly FirestoreDb _db;

    public AuthController(AuthService auth, FirestoreDb db)
    {
        _auth = auth;
        _db = db;
    }

    // =====================
    // REGISTRO
    // =====================
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
    {
        // 🔹 Registrar usuario en AuthService (ej: en memoria o EF)
        var user = await _auth.RegisterAsync(req.Email, req.Password, req.Name);

        // 🔹 Guardar también en Firestore
        var docRef = _db.Collection("usuarios").Document(user.Id);
        await docRef.SetAsync(new
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Password = req.Password, // ⚠️ en producción nunca guardar texto plano
            RegisteredAt = DateTime.UtcNow
        });

        // 🔹 Generar token
        var token = _auth.CreateJwt(user);
        return Ok(new AuthResponse(token, user.Id, user.Email, user.Name));
    }

    // =====================
    // LOGIN
    // =====================
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        // 🔹 Validar credenciales contra AuthService
        var user = await _auth.ValidateCredentialsAsync(req.Email, req.Password);

        // 🔹 (Opcional) Validar también en Firestore
        var snapshot = await _db.Collection("usuarios")
            .WhereEqualTo("Email", req.Email)
            .WhereEqualTo("Password", req.Password)
            .Limit(1)
            .GetSnapshotAsync();

        if (!snapshot.Any())
        {
            return Unauthorized("Usuario no encontrado en Firestore.");
        }

        // 🔹 Generar token
        var token = _auth.CreateJwt(user);
        return Ok(new AuthResponse(token, user.Id, user.Email, user.Name));
    }

    // =====================
    // PERFIL
    // =====================
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
