using System.Text.Json;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.Services;

public class CountryService
{
    private readonly IHttpClientFactory _factory;
    public CountryService(IHttpClientFactory factory) => _factory = factory;

    public async Task<List<CountryBasic>> GetAllAsync(string? name = null)
    {
        var client = _factory.CreateClient("restcountries");
        var path = string.IsNullOrWhiteSpace(name) ? "v3.1/all" : $"v3.1/name/{Uri.EscapeDataString(name)}";
        var res = await client.GetAsync(path);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var list = new List<CountryBasic>();
        foreach (var el in doc.RootElement.EnumerateArray())
        {
            var nameObj = el.GetProperty("name");
            string display = nameObj.TryGetProperty("common", out var c) ? c.GetString()! : nameObj.GetProperty("official").GetString()!;
            string code = el.TryGetProperty("cca2", out var cca2) ? cca2.GetString()! : (el.TryGetProperty("cca3", out var cca3) ? cca3.GetString()! : "");
            string? capital = el.TryGetProperty("capital", out var cap) && cap.ValueKind == JsonValueKind.Array && cap.GetArrayLength() > 0 ? cap[0].GetString() : null;
            string? region = el.TryGetProperty("region", out var reg) ? reg.GetString() : null;
            string? flag = el.TryGetProperty("flags", out var flags) && flags.TryGetProperty("png", out var png) ? png.GetString() : null;
            list.Add(new CountryBasic { Name = display, Code = code, Capital = capital, Region = region, FlagPng = flag });
        }
        // Simple sort by name
        return list.OrderBy(c => c.Name).ToList();
    }

    public async Task<CountryBasic?> GetByCodeAsync(string code)
    {
        var client = _factory.CreateClient("restcountries");
        var res = await client.GetAsync($"v3.1/alpha/{Uri.EscapeDataString(code)}");
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var el = doc.RootElement.EnumerateArray().FirstOrDefault();
        if (el.ValueKind == JsonValueKind.Undefined) return null;
        var nameObj = el.GetProperty("name");
        string display = nameObj.TryGetProperty("common", out var c) ? c.GetString()! : nameObj.GetProperty("official").GetString()!;
        string cca2 = el.TryGetProperty("cca2", out var cca2El) ? cca2El.GetString()! : "";
        string? capital = el.TryGetProperty("capital", out var cap) && cap.ValueKind == JsonValueKind.Array && cap.GetArrayLength() > 0 ? cap[0].GetString() : null;
        string? region = el.TryGetProperty("region", out var reg) ? reg.GetString() : null;
        string? flag = el.TryGetProperty("flags", out var flags) && flags.TryGetProperty("png", out var png) ? png.GetString() : null;
        return new CountryBasic { Name = display, Code = cca2, Capital = capital, Region = region, FlagPng = flag };
    }
}