namespace EcoTrip.Models.DtoS
{
    public class CreateBookingDto
    {
        public int TripId { get; set; }
        public int Seats { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentType { get; set; }
    }
}
