namespace AgenciaViajes.API.DTOs
{
    public class RegisterRequest
    {
        public string ClientId { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? DestinationCountryCode { get; set; } // opcional
    }
}
