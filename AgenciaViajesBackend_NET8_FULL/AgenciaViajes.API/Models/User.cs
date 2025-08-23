namespace AgenciaViajes.API.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; } = default!;
    public string Name { get; set; } = "";
    public string PasswordHash { get; set; } = default!;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}