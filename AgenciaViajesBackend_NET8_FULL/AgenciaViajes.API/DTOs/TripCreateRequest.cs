
public class TripCreateRequest
{
    public string Title { get; set; } = "";
    public string CountryCode { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Planned";
    public string? Description { get; set; }
}