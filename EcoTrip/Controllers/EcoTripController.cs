using EcoTrip.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Öko utak lekérése kártyára
        /// </summary>
        /// <remarks>
        /// Összes ökoút adatai kártyára
        /// </remarks>
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
                             ImageUrl = t.image_url
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
                        city = h.City,
                        hotel_name = h.HotelName,
                        stars = h.Stars,
                        image_url = h.ImageUrl
                    }).ToList()
                })
                .ToList();

            return Ok(new { result = grouped });
        }


        /// <summary>
        /// Öko utak lekérése id alapján
        /// </summary>
        /// <remarks>
        /// Egy út adata
        /// </remarks>
        [HttpGet("detailed/{id}")]
        public ActionResult GetEcoTripDetails(int id)
        {
            var result = _context.trips
                .Include(t => t.TripRoutes)
                .Include(t => t.TripsImages)
                .Where(t => t.id == id && t.type == 1)
                .Select(t => new
                {
                    id = t.id,
                    main_image = t.image_url,
                    city = t.city,
                    hotel_name = t.hotel_name,
                    stars = t.stars,
                    price = t.price,
                    long_description = t.long_description,

                    gallery_images = t.TripsImages
                        .Select(img => img.image_url)
                        .ToList(),
                    routes = t.TripRoutes
                        .Select(r => r.route_text)
                        .ToList()
                })
                .FirstOrDefault();

            if (result == null)
                return NotFound("Nincs ilyen eco trip");

            return Ok(result);
        }
    }
}
