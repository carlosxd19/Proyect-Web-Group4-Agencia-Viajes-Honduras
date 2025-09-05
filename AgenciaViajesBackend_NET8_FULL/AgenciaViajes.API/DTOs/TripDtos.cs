namespace AgenciaViajes.API.DTOs
{
    public class TripDto
    {
        public string Id { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string CountryCode { get; set; } = default!;
        public string City { get; set; } = default!;
        public int StayDays { get; set; }
        public string PassengerType { get; set; } = default!;
        public DateTime? TravelDate { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
