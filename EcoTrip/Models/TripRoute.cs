using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoTrip.Models
{
    [Table("trip_routes")]
    public class TripRoute
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int trip_id { get; set; }

        [Required]
        [StringLength(255)]
        public string route_text { get; set; }

        // Navigációs tulajdonság a szülő Triphez
        [ForeignKey("trip_id")]
        public virtual Trips Trip { get; set; }
    }
}
