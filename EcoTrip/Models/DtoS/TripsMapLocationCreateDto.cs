using System.ComponentModel.DataAnnotations;

namespace EcoTrip.Models.DtoS
{
    public class TripsMapLocationCreateDto
    {
        [Required]
        public int TripsId { get; set; }  // Trips_id

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }
    }
}
