using System.ComponentModel.DataAnnotations.Schema;

namespace EcoTrip.Models
{
    [Table("trips_images")]
    public class TripsImage
    {
        public int id { get; set; }
        public int trip_id { get; set; }
        public string image_url { get; set; }

        [ForeignKey("trip_id")] 
        public virtual Trips Trip { get; set; }
    }
}
