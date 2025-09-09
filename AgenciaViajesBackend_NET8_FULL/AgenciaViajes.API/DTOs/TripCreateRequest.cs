using System;
using System.ComponentModel.DataAnnotations;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.DTOs
{
    public class TripCreateRequest
    {
        [Required] public string ClientId { get; set; } = default!;
        [Required] public string CountryCode { get; set; } = default!;
        [Required] public string City { get; set; } = default!;
        [Range(1, 365)] public int StayDays { get; set; } = 1;
        [Required] public PassengerType PassengerType { get; set; }
        [Required] public DateTime TravelDate { get; set; }
    }
}
