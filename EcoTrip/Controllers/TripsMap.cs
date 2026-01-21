using EcoTrip.Models;
using EcoTrip.Models.DtoS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsMap : ControllerBase
    {
        private readonly EcoTripDbContext _context;

        public TripsMap(EcoTripDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Sima utak térképes adatainak lekérése
        /// </summary>
        /// <remarks>
        /// Marker adatai
        /// </remarks>
        [HttpGet("Sima")]
        public async Task<IActionResult> GetNormalTrips()
        {
            var locations = await _context.TripsMapLocations
                .Join(
                    _context.trips,
                    map => map.Trips_id,
                    trip => trip.id,
                    (map, trip) => new { map, trip }
                )
                .Join(
                    _context.countrys,
                    mt => mt.trip.country_id,
                    country => country.id,
                    (mt, country) => new { mt.map, mt.trip, country }
                )
                .Where(x => x.trip.type == 0)
                .Select(x => new TripsMapLocationDto
                {
                    Country = x.country.country,
                    City = x.trip.city,
                    HotelName = x.trip.hotel_name,
                    Stars = x.trip.stars,
                    Description = x.map.Description,
                    Latitude = x.map.Latitude,
                    Longitude = x.map.Longitude
                })
                .ToListAsync();

            return Ok(locations);
        }

        /// <summary>
        /// Sima utak térképhez adása.
        /// </summary>
        /// <remarks>
        /// Sima utak térkép postja.
        /// </remarks>
        [HttpPost("Sima")]
        public async Task<IActionResult> CreateNormalMapLocation([FromBody] TripsMapLocationCreateDto dto)
        {
            if (dto.Latitude < -90 || dto.Latitude > 90 || dto.Longitude < -180 || dto.Longitude > 180)
            {
                return BadRequest("Érvénytelen koordináták.");
            }

            var trip = await _context.trips
                .FirstOrDefaultAsync(t => t.id == dto.TripsId && t.type == 0);

            if (trip == null)
            {
                return BadRequest("Nem létezik sima út ezzel az ID-val.");
            }

            var newLocation = new TripsMapLocations
            {
                Trips_id = dto.TripsId,
                Description = dto.Description,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Created_at = DateTime.UtcNow
            };

            _context.TripsMapLocations.Add(newLocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNormalTrips), new { }, dto);
        }


        /// <summary>
        /// Ökó utak térképes adatainak lekérése
        /// </summary>
        /// <remarks>
        /// Marker adatai
        /// </remarks>
        [HttpGet("Eco")]
        public async Task<IActionResult> GetEcoTrips()
        {
            var locations = await _context.TripsMapLocations
                .Join(
                    _context.trips,
                    map => map.Trips_id,
                    trip => trip.id,
                    (map, trip) => new { map, trip }
                )
                .Join(
                    _context.countrys,
                    mt => mt.trip.country_id,
                    country => country.id,
                    (mt, country) => new { mt.map, mt.trip, country }
                )
                .Where(x => x.trip.type == 1)
                .Select(x => new TripsMapLocationDto
                {
                    Country = x.country.country,
                    City = x.trip.city,
                    HotelName = x.trip.hotel_name,
                    Stars = x.trip.stars,
                    Description = x.map.Description,
                    Latitude = x.map.Latitude,
                    Longitude = x.map.Longitude
                })
                .ToListAsync();

            return Ok(locations);
        }


        /// <summary>
        /// Öko utak térképhez adása.
        /// </summary>
        /// <remarks>
        /// Öko utak térkép postja.
        /// </remarks>
        [HttpPost("Eco")]
        public async Task<IActionResult> CreateEcoMapLocation([FromBody] TripsMapLocationCreateDto dto)
        {
            if (dto.Latitude < -90 || dto.Latitude > 90 || dto.Longitude < -180 || dto.Longitude > 180)
            {
                return BadRequest("Érvénytelen koordináták.");
            }

            var trip = await _context.trips
                .FirstOrDefaultAsync(t => t.id == dto.TripsId && t.type == 1);

            if (trip == null)
            {
                return BadRequest("Nem létezik öko út ezzel az ID-val.");
            }

            var newLocation = new TripsMapLocations
            {
                Trips_id = dto.TripsId,
                Description = dto.Description,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Created_at = DateTime.UtcNow
            };

            _context.TripsMapLocations.Add(newLocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEcoTrips), new { }, dto);
        }



    }
}
