using EcoTrip.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public class UpdateProfileImageDto
        {
            public string ImageUrl { get; set; }
        }

    }

}
