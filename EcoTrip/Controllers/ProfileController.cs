using EcoTrip.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EcoTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly EcoTripDbContext _context;

        public ProfileController(EcoTripDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Username put
        /// </summary>
        /// <remarks>
        /// Username put
        /// </remarks>
        [HttpPut("username")]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Érvénytelen token.");

            if (string.IsNullOrWhiteSpace(dto.UserName))
                return BadRequest("Felhasználónév kötelező.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound("Felhasználó nem található.");

            var newUserName = dto.UserName.Trim();

            var exists = await _context.Users
                .AnyAsync(u => u.Username == newUserName && u.Id != userId);

            if (exists)
                return BadRequest("Ez a felhasználónév már foglalt.");

            user.Username = newUserName;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Felhasználónév frissítve",
                userName = user.Username
            });
        }


        /// <summary>
        /// Password put
        /// </summary>
        /// <remarks>
        /// Password put
        /// </remarks>
        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Érvénytelen token.");

            if (string.IsNullOrWhiteSpace(dto.OldPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("Régi és új jelszó kötelező.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound("Felhasználó nem található.");

            var hasher = new PasswordHasher<Users>();

            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.OldPassword);
            if (result == PasswordVerificationResult.Failed)
                return BadRequest("Hibás régi jelszó.");

            user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Jelszó sikeresen frissítve"
            });
        }







        /// <summary>
        /// Profilkép post
        /// </summary>
        /// <remarks>
        /// Profilkép feltöltése
        /// </remarks>
        [HttpPut("image")]
        public async Task<IActionResult> UpdateProfileImage([FromBody] UpdateProfileImageDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Érvénytelen token.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("Felhasználó nem található.");
            }

            if (string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                return BadRequest("Kép URL kötelező.");
            }

            user.ProfileImage = dto.ImageUrl.Trim();
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profilkép frissítve",
                profileImage = user.ProfileImage
            });
        }


        /// <summary>
        /// Profil adatok get
        /// </summary>
        /// <remarks>
        /// Profil adatok lekérése
        /// </remarks>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Érvénytelen vagy hiányzó felhasználói azonosító.");
            }

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.ProfileImage
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Érvénytelen token.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("Felhasználó nem található.");
            }

            var userBookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .ToListAsync();

            if (userBookings.Any())
            {
                _context.Bookings.RemoveRange(userBookings);
            }


            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                
                return StatusCode(500, "Nem sikerült törölni a fiókot – valószínűleg még más kapcsolódó adatok léteznek az adatbázisban. " +
                                       "Próbáld később, vagy vedd fel velünk a kapcsolatot.");
            }

            return Ok(new
            {
                message = "Fiókod, az összes foglalásod és értékelésed sikeresen törölve."
            });
        }


        public class UpdateUsernameDto
        {
            public string UserName { get; set; }
        }

        public class UpdatePasswordDto
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }



        public class UpdateProfileImageDto
        {
            public string ImageUrl { get; set; }
        }

    }

}
