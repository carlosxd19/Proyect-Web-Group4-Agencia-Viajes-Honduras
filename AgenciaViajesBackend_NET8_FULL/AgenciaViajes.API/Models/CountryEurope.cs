using System.Text.Json.Serialization;

namespace AgenciaViajes.API.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CountryEurope
    {
        España,
        Francia,
        Alemania,
        Italia,
        Portugal,
        ReinoUnido,
        PaisesBajos,
        Belgica,
        Suiza,
        Suecia
    }
}
