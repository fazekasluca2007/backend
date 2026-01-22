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
            if (review == null) return BadRequest("Review objektum hiányzik");

            _context.Reviews.Add(review);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = review.Id }, review);
        }


        /// <summary>
        /// Vélemény módositása
        /// </summary>
        /// <remarks>
        /// Vélemény put
        /// </remarks>
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Reviews updatedReview)
        {
            if (updatedReview == null)
                return BadRequest("Hiányzik a review objektum");

            var existing = _context.Reviews.Find(id);
            if (existing == null)
                return NotFound();


            bool modified = false;

            if (!string.IsNullOrWhiteSpace(updatedReview.Name) &&
                updatedReview.Name != existing.Name)
            {
                existing.Name = updatedReview.Name;
                modified = true;
            }

            if (!string.IsNullOrWhiteSpace(updatedReview.Review) &&
                updatedReview.Review != existing.Review)
            {
                existing.Review = updatedReview.Review;
                modified = true;
            }

            if (updatedReview.Stars != 0 && updatedReview.Stars != existing.Stars)
            {
                existing.Stars = updatedReview.Stars;
                modified = true;
            }

            if (!modified)
                return BadRequest("Nem adtál meg egyetlen módosítandó értéket sem");

            _context.SaveChanges();
            return NoContent();
        }


        /// <summary>
        /// Vélemény törlése
        /// </summary>
        /// <remarks>
        /// Vélemény delete
        /// </remarks>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var review = _context.Reviews.Find(id);

            if (review == null)
                return NotFound($"Nem található vélemény ID={id}");

            _context.Reviews.Remove(review);
            _context.SaveChanges();

            return NoContent(); 
        }

    }
}
