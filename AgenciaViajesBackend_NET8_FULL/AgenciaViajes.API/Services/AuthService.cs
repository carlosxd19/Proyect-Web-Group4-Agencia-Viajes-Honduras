using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AgenciaViajes.API.Models;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using BCryptNet = BCrypt.Net.BCrypt;

namespace AgenciaViajes.API.Services;

public class AuthService
{
    private readonly FirebaseService _firebase;
    private readonly IConfiguration _config;
    public AuthService(FirebaseService firebase, IConfiguration config)
    {
        _firebase = firebase;
        _config = config;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var ss = await _firebase.Users.WhereEqualTo("Email", email).Limit(1).GetSnapshotAsync();
        return ss.Documents.Select(d => d.ConvertTo<User>()).FirstOrDefault();
    }

    public async Task<User> RegisterAsync(string email, string password, string name)
    {
        var existing = await GetUserByEmail(email);
        if (existing != null) throw new InvalidOperationException("Email ya está registrado.");
        var user = new User { Email = email, Name = name, PasswordHash = BCryptNet.HashPassword(password) };
        await _firebase.Users.Document(user.Id).SetAsync(user);
        return user;
    }

    public async Task<User> ValidateCredentialsAsync(string email, string password)
    {
        var user = await GetUserByEmail(email) ?? throw new UnauthorizedAccessException("Credenciales inválidas.");
        if (!BCryptNet.Verify(password, user.PasswordHash)) throw new UnauthorizedAccessException("Credenciales inválidas.");
        return user;
    }

    public string CreateJwt(User user)
    {
        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name ?? "")
        };
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiresMinutes"] ?? "120")),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}