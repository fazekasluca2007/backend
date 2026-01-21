namespace EcoTrip.Models.DtoS
{
    public class TripsMapLocationDto
    {
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public int Stars { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
