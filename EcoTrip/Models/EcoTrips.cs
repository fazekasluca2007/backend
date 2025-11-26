using System.ComponentModel.DataAnnotations;

namespace EcoTrip.Models
{
    public class EcoTrips
    {
        public int id { get; set; }
        [Required]
        public string country { get; set; }
        [Required]
        public string country_description { get; set; }

        [Required]
        public string city { get; set; }
        [Required]
        public string hotel_name {  get; set; }
        [Required]
        public int stars { get; set; }
        [Required]
        public string description {  get; set; }
        [Required]
        public string services {  get; set; }
        [Required]
        public string image_url {  get; set; }
        [Required]
        public string modalId {  get; set; }
        [Required]
        public int price { get; set; }
    }
}
