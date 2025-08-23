using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.DTOs;

public record TripCreateRequest(string Title, string CountryCode, DateTime StartDate, DateTime EndDate, TripStatus Status, string? Description);
public record TripUpdateRequest(string Title, string CountryCode, DateTime StartDate, DateTime EndDate, TripStatus Status, string? Description);