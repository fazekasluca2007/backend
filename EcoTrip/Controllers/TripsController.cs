using EcoTrip.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly EcoTripDbContext _context;

        public TripsController()
        {
            _context = new EcoTripDbContext();
        }

        //Trips oldal kártyáihoz
        [HttpGet("tripcards")]
        public ActionResult GetTripsBasic()
        {
            var trips = (from t in _context.trips
                         join c in _context.countrys on t.country_id equals c.id
                         where t.type == 0
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


        //Modal ablak adatai
        [HttpGet("modal/{id}")]
        public ActionResult GetTripModal(int id)
        {
            var result = _context.trips
                .Include(t => t.TripsImages)  // Betöltjük a galéria képeket
                .Where(t => t.id == id && t.type == 0)
                .Select(t => new
                {
                    id = t.id,
                    main_image = t.image_url,  // fő kép (kártyán látható)
                    city = t.city,
                    hotel_name = t.hotel_name,
                    stars = t.stars,
                    short_description = t.short_description,
                    //Kepek a szallasokrol
                    gallery_images = t.TripsImages
                        .Select(img => img.image_url)
                        .ToList()
                })
                .FirstOrDefault();

            if (result == null)
                return NotFound("Nincs ilyen trip");

            return Ok(result);
        }

        //Külön oldal adatai
        [HttpGet("detailed/{id}")]
        public ActionResult GetTripDetails(int id)
        {
            var result = _context.trips
                .Include(t => t.TripRoutes)       // Betöltjük a kapcsolódó útvonalakat
                .Include(t => t.TripsImages)
                .Where(t => t.id == id && t.type == 0)
                .Select(t => new
                {
                    id = t.id,
                    main_image = t.image_url,
                    city = t.city,
                    hotel_name = t.hotel_name,
                    stars = t.stars,
                    price = t.price,
                    long_description = t.long_description,
                    services = t.services ?? "",

                    gallery_images = t.TripsImages
                        .Select(img => img.image_url)
                        .ToList(),
                    routes = t.TripRoutes
                        .Select(r => r.route_text)
                        .ToList()
                })
                .FirstOrDefault();

            if (result == null)
                return NotFound("Nincs ilyen trip");

            return Ok(result);
        }
    }
}
