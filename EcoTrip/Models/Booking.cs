using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoTrip.Models
{
    [Table("bookings")]
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("trip_id")]
        public int? TripId { get; set; }

        public int Seats { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        public string Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
