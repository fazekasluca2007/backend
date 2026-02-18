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


            var user = await _context.Users
                .AsNoTracking()
                .Select(u => new { u.Id, u.Email })
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized("Felhasználó nem található.");

            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new MyBookingResponseDto
                {
                    Id = b.Id,
                    TripId = b.TripId,
                    HotelName = _context.trips
                        .Where(t => t.id == b.TripId)
                        .Select(t => t.hotel_name)
                        .FirstOrDefault() ?? "Ismeretlen",
                    Seats = b.Seats,
                    Days = b.Days,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    PaymentType = b.PaymentType,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,

                    UserEmail = user.Email
                })
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

            if (dto.StartDate == default || dto.EndDate == default)
                return BadRequest("Kezdő és záró dátum megadása kötelező!");

            if (dto.EndDate <= dto.StartDate)
                return BadRequest("A záró dátumnak későbbinek kell lennie, mint a kezdő dátum!");

            var days = (dto.EndDate.Date - dto.StartDate.Date).Days;
            if (days <= 0)
                return BadRequest("Legalább 1 napos foglalás szükséges!");

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Érvénytelen token.");

            var trip = await _context.trips.FindAsync(dto.TripId);

            if (trip == null)
                return NotFound("Utazás nem található.");

            decimal totalPrice = trip.price * days * dto.Seats;

            var booking = new Booking
            {
                UserId = userId,
                TripId = dto.TripId,
                Seats = dto.Seats,
                Days = days,
                StartDate = dto.StartDate.Date,
                EndDate = dto.EndDate.Date,
                PaymentType = dto.PaymentType,
                TotalPrice = totalPrice,
                Status = "pending"
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound("Foglalás nem található.");


            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Érvénytelen token.");

            if (booking.UserId != userId && !User.IsInRole("Admin"))
                return Forbid("Nincs jogosultság a foglalás törléséhez.");

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
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string PaymentType { get; set; }
        }

        public class UpdateBookingStatusDto
        {
            public string Status { get; set; }
        }

        public class MyBookingResponseDto
        {
            public int Id { get; set; }
            public int TripId { get; set; }
            public string HotelName { get; set; }
            public int Seats { get; set; }
            public int Days { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string PaymentType { get; set; }
            public decimal TotalPrice { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
            public string UserEmail { get; set; }
        }
    }
}
