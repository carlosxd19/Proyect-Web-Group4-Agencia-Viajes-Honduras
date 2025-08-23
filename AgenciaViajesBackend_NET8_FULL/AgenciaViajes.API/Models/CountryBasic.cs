namespace AgenciaViajes.API.Models;

public class CountryBasic
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!; // cca2 or cca3
    public string? Capital { get; set; }
    public string? Region { get; set; }
    public string? FlagPng { get; set; }
}