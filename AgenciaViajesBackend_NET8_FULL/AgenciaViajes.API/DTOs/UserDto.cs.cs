namespace AgenciaViajes.API.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PreferredCountryCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
