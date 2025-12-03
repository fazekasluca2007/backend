using EcoTrip.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var result = (from t in _context.trips
                          join c in _context.countrys on t.country_id equals c.id
                          select new
                          {
                              id = t.id,
                              country = c.country,
                              country_description = c.country_description,
                              city = t.city,
                              hotel_name = t.hotel_name,
                              stars = t.stars,
                              image_url = t.image_url
                          }).ToList();

            return Ok(result);
        }


        //Modal ablak adatai
        [HttpGet("modal")]
        public ActionResult GetTripsList()
        {
            var result = _context.trips
                .Select(t => new
                {
                    image_url = t.image_url,
                    city = t.city,
                    hotel_name = t.hotel_name,
                    stars = t.stars
                }).ToList();

            return Ok(result);
        }

        //Külön oldal adatai
        [HttpGet("detailed")]
        public ActionResult GetTripsDetails()
        {
            var result = _context.trips
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
