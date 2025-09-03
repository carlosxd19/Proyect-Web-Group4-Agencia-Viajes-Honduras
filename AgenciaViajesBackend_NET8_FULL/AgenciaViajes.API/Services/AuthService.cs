using Google.Cloud.Firestore;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using AgenciaViajes.API.DTOs;
using BCrypt.Net;

namespace AgenciaViajes.API.Services;

public class AuthService
{
    private readonly FirestoreDb _db;
    private readonly IConfiguration _config;

    public AuthService(FirestoreDb db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // =========================
    // Registrar usuario
    // =========================
    public async Task<UserDto> RegisterAsync(string email, string password, string name)
    {
        var usersRef = _db.Collection("usuarios");

        // Revisar si ya existe
        var query = await usersRef.WhereEqualTo("Email", email).GetSnapshotAsync();
        if (query.Any())
            throw new Exception("El correo ya está registrado");

        // Hash de la contraseña
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var newUser = new UserDto
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            Name = name,
            Password = hashedPassword,
            RegisteredAt = DateTime.UtcNow
        };

        await usersRef.Document(newUser.Id).SetAsync(newUser);

        return newUser;
    }

    // =========================
    // Validar credenciales
    // =========================
    public async Task<UserDto> ValidateCredentialsAsync(string email, string password)
    {
        var usersRef = _db.Collection("usuarios");
        var query = await usersRef.WhereEqualTo("Email", email).GetSnapshotAsync();

        if (!query.Any())
            throw new Exception("Usuario no encontrado");

        var doc = query.First();
        var user = doc.ConvertTo<UserDto>();

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new Exception("Contraseña incorrecta");

        return user;
    }

    // =========================
    // Crear JWT
    // =========================
    public string CreateJwt(UserDto user)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // =========================
    // Obtener usuario por email
    // =========================
    public async Task<UserDto?> GetUserByEmail(string email)
    {
        var usersRef = _db.Collection("usuarios");
        var query = await usersRef.WhereEqualTo("Email", email).GetSnapshotAsync();

        return query.Any() ? query.First().ConvertTo<UserDto>() : null;
    }
}
