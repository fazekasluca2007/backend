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
                    return Ok(new { message = "Sikeres lekérdezés", result = context.eco_trips.ToList() });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, result = "" });
            }
        }
    }
}
