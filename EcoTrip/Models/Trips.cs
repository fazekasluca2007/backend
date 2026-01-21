using System.ComponentModel.DataAnnotations;

namespace EcoTrip.Models
{
    public class Trips
    {
        public int id { get; set; }

        public int type { get; set; } 
        public int country_id { get; set; }

        [Required]
        public string city { get; set; }

        [Required]
        public string hotel_name { get; set; }

        public int stars { get; set; }

        public string short_description { get; set; }
        public string long_description { get; set; }

        public string services { get; set; }

        public string image_url { get; set; }

        public string modalId { get; set; }

        public decimal price { get; set; }

        public int available { get; set; }
        public virtual ICollection<TripRoute> TripRoutes { get; set; } = new List<TripRoute>();
        public virtual ICollection<TripsImage> TripsImages { get; set; } = new List<TripsImage>();
    }
}
