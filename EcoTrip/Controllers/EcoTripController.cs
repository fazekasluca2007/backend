using EcoTrip.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcoTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EcoTripController : ControllerBase
    {
        private readonly EcoTripDbContext _context;

        public EcoTripController()
        {
            _context = new EcoTripDbContext();
        }

        // Ecotrips oldal kártyáihoz
        [HttpGet("ecotripcards")]
        public ActionResult GetEcoTripsBasic()
        {
            var trips = (from t in _context.trips
                         join c in _context.countrys on t.country_id equals c.id
                         where t.type == 1
                         select new
                         {
                             TripId = t.id,
                             Country = c.country,
                             CountryDescription = c.country_description,
                             FlagUrl = c.flag_url,
                             City = t.city,
                             HotelName = t.hotel_name,
                             Stars = t.stars,
                             ImageUrl = t.image_url,
                             ModalId = t.modalId
                         })
                         .ToList();

            var grouped = trips
                .GroupBy(x => x.Country)
                .Select(g => new
                {
                    country = g.Key,
                    flag = g.First().FlagUrl,
                    description = g.First().CountryDescription,
                    hotels = g.Select(h => new
                    {
                        id = h.TripId,
                        modalId = h.ModalId,
                        city = h.City,
                        hotel_name = h.HotelName,
                        stars = h.Stars,
                        image_url = h.ImageUrl
                    }).ToList()
                })
                .ToList();

            return Ok(new { result = grouped });
        }

        // Modal ablak adatai
        [HttpGet("modal/{id}")]
        public ActionResult GetEcoTripModal(int id)
        {
            var result = _context.trips
                .Where(t => t.id == id && t.type == 1)
                .Select(t => new
                {
                    id = t.id,
                    image_url = t.image_url,
                    city = t.city,
                    hotel_name = t.hotel_name,
                    stars = t.stars
                })
                .FirstOrDefault();

            if (result == null)
                return NotFound("Nincs ilyen eco trip");

            return Ok(result);
        }

        // Külön ablak adatai
        [HttpGet("detailed/{id}")]
        public ActionResult GetEcoTripDetails(int id)
        {
            var result = _context.trips
                .Where(t => t.id == id && t.type == 1)
                .Select(t => new
                {
                    id = t.id,
                    image_url = t.image_url,
                    city = t.city,
                    hotel_name = t.hotel_name,
                    stars = t.stars,
                    long_description = t.long_description,
                    services = t.services,

                    images = _context.trips_images
                        .Where(img => img.trip_id == t.id)
                        .Select(img => img.image_url)
                        .ToList()
                })
                .FirstOrDefault();

            if (result == null)
                return NotFound("Nincs ilyen eco trip");

            return Ok(result);
        }
    }
}
