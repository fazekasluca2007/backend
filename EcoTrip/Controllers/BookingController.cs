using EcoTrip.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        /// Get bookings by token user id
        /// </summary>
        /// <remarks>
        /// Get bookings by token user id
        /// </remarks>
        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Érvénytelen token.");

            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

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
                return BadRequest("A helyek számának 0-nál nagyobbnak kell lennie!");

            if (dto.Days <= 0)
                return BadRequest("A napok számának 0-nál nagyobbnak kell lennie!");

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Érvénytelen token.");

            var trip = await _context.trips.FindAsync(dto.TripId);

            if (trip == null)
                return NotFound("Trip nem található.");

            decimal totalPrice = trip.price * dto.Days * dto.Seats;

            var booking = new Booking
            {
                UserId = userId,
                TripId = dto.TripId,
                Seats = dto.Seats,
                Days = dto.Days,
                PaymentType = dto.PaymentType,
                TotalPrice = totalPrice,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Foglalás létrehozva",
                bookingId = booking.Id,
                totalPrice = booking.TotalPrice
            });
        }

        /// <summary>
        /// Booking delete by id
        /// </summary>
        /// <remarks>
        /// Booking delete by id
        /// </remarks>
        [Authorize(Roles = "Admin")]
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
            public int TripId { get; set; }
            public int Seats { get; set; }
            public int Days { get; set; }
            public string PaymentType { get; set; }
        }

        public class UpdateBookingStatusDto
        {
            public string Status { get; set; }
        }
    }
}
