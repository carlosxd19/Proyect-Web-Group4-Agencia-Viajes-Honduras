using System.Text.Json.Serialization;

namespace AgenciaViajes.API.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Region
    {
        Europa = 1
    }
}
