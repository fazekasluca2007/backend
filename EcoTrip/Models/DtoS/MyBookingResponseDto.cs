namespace EcoTrip.Models.DtoS
{
    public class MyBookingResponseDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string HotelName { get; set; }
        public int Seats { get; set; }
        public int Days { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentType { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserEmail { get; set; }
    }
}
