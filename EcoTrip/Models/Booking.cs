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

        [Column("seats")]
        public int Seats { get; set; }
        [Column("days")]
        public int Days { get; set; }
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }
        [Column("payment_type")]
        public string PaymentType { get; set; }

        public string Status { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
