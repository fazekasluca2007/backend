using EcoTrip.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly EcoTripDbContext _context;

        public BookingsController(EcoTripDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Get bookings by user id
        /// </summary>
        /// <remarks>
        /// Get bookings by user id
        /// </remarks>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserBookings(int userId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            if (!bookings.Any())
                return NotFound("Nincs foglalás ehhez a felhasználóhoz.");

            return Ok(bookings);
        }

        /// <summary>
        /// Booking post
        /// </summary>
        /// <remarks>
        /// Booking post
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            if (dto.Seats <= 0)
                return BadRequest("Seats must be greater than 0.");

            var booking = new Booking
            {
                UserId = dto.UserId,
                TripId = dto.TripId,
                Seats = dto.Seats,
                TotalPrice = dto.TotalPrice,
                Status = "pending"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Foglalás létrehozva",
                bookingId = booking.Id
            });
        }

        /// <summary>
        /// Booking delete
        /// </summary>
        /// <remarks>
        /// Booking delete
        /// </remarks>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound("Foglalás nem található.");

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok("Foglalás törölve.");
        }

        /// <summary>
        /// Booking update status
        /// </summary>
        /// <remarks>
        /// Booking update status
        /// </remarks>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound("Foglalás nem található.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest("Status kötelező.");

            booking.Status = dto.Status.Trim();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Státusz frissítve",
                booking.Id,
                booking.Status
            });
        }
        public class CreateBookingDto
        {
            public int UserId { get; set; }
            public int? TripId { get; set; }
            public int Seats { get; set; }
            public decimal TotalPrice { get; set; }
        }

        public class UpdateBookingStatusDto
        {
            public string Status { get; set; }
        }
    }
}
