using AgenciaViajes.API.Models;

public class TripCreateRequest
{
    public string ClientId { get; set; } = default!;
    public string CountryCode { get; set; } = default!;
    public string City { get; set; } = default!;
    public int StayDays { get; set; }
    public PassengerType PassengerType { get; set; } = PassengerType.Adult;
    public DateTime? TravelDate { get; set; }
}