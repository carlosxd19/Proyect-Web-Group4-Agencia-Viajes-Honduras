namespace AgenciaViajes.API.Models;

public class FavoriteCountry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = default!;
    public string CountryCode { get; set; } = default!; // ISO alpha-2 or alpha-3
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}