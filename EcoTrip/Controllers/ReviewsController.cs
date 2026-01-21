using EcoTrip.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly EcoTripDbContext _context;

        public ReviewsController()
        {
            _context = new EcoTripDbContext();
        }


        /// <summary>
        /// Vélemények lekérése
        /// </summary>
        /// <remarks>
        /// Vélemény get
        /// </remarks>
        [HttpGet]
        public IActionResult Get()
        {
            var list = _context.Reviews
                .ToList();
            return Ok(list);
        }

        /// <summary>
        /// Vélemény postolása
        /// </summary>
        /// <remarks>
        /// Vélemény post
        /// </remarks>
        [HttpPost]
        public IActionResult Post([FromBody] Reviews review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = review.Id }, review);
        }


    }
}
