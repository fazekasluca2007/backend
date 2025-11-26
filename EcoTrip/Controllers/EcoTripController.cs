using EcoTrip.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcoTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EcoTripController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetAllEcoTrips()
        {
            try
            {
                using (var context = new EcoTripDbContext())
                {
                    var raw = context.eco_trips.ToList();


                    var grouped = raw
                        .GroupBy(e => e.country)
                        .Select(g => new
                        {
                            country = g.Key,
                            flag = g.Key == "Magyarország" ? "img/zaszlok/hu.png" : "img/zaszlok/it.png",
                            description = g.First().country_description,
                            hotels = g.Select(h => new
                            {
                                city = h.city,
                                name = h.hotel_name,
                                stars = h.stars,
                                img = h.image_url,
                                modalId = h.modalId,
                                description = h.description,
                                services = h.services,
                                price = h.price
                            }).ToList()
                        })
                        .ToList();

                    return Ok(new { message = "Sikeres lekérdezés", result = grouped });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
