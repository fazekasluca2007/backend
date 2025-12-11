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
            var trips = (from t in _context.eco_trips
                         join c in _context.countrys on t.country_id equals c.id
                         select new
                         {
                             TripId = t.id,
                             Country = c.country,
                             CountryDescription = c.country_description,
                             FlagUrl = c.flag_url,         // <-- Most közvetlenül az adatbázisból
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
                    flag = g.First().FlagUrl ?? "https://flagcdn.com/256x192/xx.png", // fallback, ha NULL
                    description = g.First().CountryDescription,
                    hotels = g.Select(h => new
                    {
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
        [HttpGet("modal")]
        public ActionResult GetEcoTripsList()
        {
            var result = _context.eco_trips
                .Select(t => new
                {
                    image_url = t.image_url,
                    city = t.city,
                    hotel_name = t.hotel_name,
                    stars = t.stars
                }).ToList();

            return Ok(result);
        }

        // Külön ablak adatai
        [HttpGet("detailed")]
        public ActionResult GetEcoTripsDetails()
        {
            var result = _context.eco_trips
                .Select(t => new
                {
                    image_url = t.image_url,
                    city = t.city,
                    hotel_name = t.hotel_name,
                    stars = t.stars,
                    long_description = t.long_description,
                    services = t.services
                }).ToList();

            return Ok(result);
        }
    }
}
