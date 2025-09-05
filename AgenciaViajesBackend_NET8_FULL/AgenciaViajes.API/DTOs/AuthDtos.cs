namespace AgenciaViajes.API.DTOs
{
    public class AuthResponse
    {
        public string ClientId { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? DestinationCountryCode { get; set; }
    }
}
