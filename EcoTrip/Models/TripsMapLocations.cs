using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoTrip.Models
{
    [Table("trip_map_locations")]
    public class TripsMapLocations
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Trips_id { get; set; }

        [Required]
        public string Description { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal Longitude { get; set; }

        public DateTime Created_at { get; set; }
    }
}
