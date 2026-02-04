using EcoTrip.Models;
using EcoTrip.Models.DtoS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EcoTrip.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EcoTripDbContext _context;
        private readonly PasswordHasher<Users> _hasher = new();

        public AuthController()
        {
            _context = new EcoTripDbContext();
        }


        private string GenerateJwtToken(Users user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("EcoTrip_Dev_Secret_Key_123456789_ABC_xyz")
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "EcoTrip",
                audience: "EcoTripClient",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        /// <summary>
        /// Regisztráció post
        /// </summary>
        /// <remarks>
        /// Regisztráció
        /// </remarks>
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email már létezik");

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Felhasználónév már létezik");

            var user = new Users
            {
                FullName = dto.FullName,
                Username = dto.Username,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Sikeres regisztráció");
        }


        /// <summary>
        /// Bejelentkezés post
        /// </summary>
        /// <remarks>
        /// Bejelentkezés
        /// </remarks>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Username);

            if (user == null)
                return Unauthorized("Hibás felhasználónév vagy jelszó");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Hibás felhasználónév vagy jelszó");

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Username,
                    user.Email
                }
            });
        }



    }

}