using System.ComponentModel.DataAnnotations;

namespace EcoTrip.Models
{
    public class Country
    {
        public int id { get; set; }

        [Required]
        public string country { get; set; }

        [Required]
        public string country_description { get; set; }
        public string? flag_url { get; set; } 
    }
}
