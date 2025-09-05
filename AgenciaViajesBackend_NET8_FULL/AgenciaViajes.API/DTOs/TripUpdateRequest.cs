using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.DTOs
{
    public class TripUpdateRequest
    {
        public string CountryCode { get; set; } = default!;
        public string City { get; set; } = default!;
        public int StayDays { get; set; }
        public PassengerType PassengerType { get; set; }
        public DateTime? TravelDate { get; set; }
        public TripStatus Status { get; set; } = TripStatus.Pending;
    }
}
