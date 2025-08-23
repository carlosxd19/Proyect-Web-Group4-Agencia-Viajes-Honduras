namespace AgenciaViajes.API.Models;

public enum TripStatus { Planificado, Completado }

public class Trip
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string CountryCode { get; set; } = default!; // e.g., "US", "HN"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TripStatus Status { get; set; } = TripStatus.Planificado;
    public string? Description { get; set; }
}