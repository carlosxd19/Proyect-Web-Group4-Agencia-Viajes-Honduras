using System.ComponentModel.DataAnnotations;
using AgenciaViajes.API.Models;

namespace AgenciaViajes.API.DTOs
{
    public class CountrySelectionRequest
    {
        [Required] public CountryEurope Country { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
