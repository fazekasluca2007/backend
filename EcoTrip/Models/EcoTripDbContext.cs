using Microsoft.EntityFrameworkCore;

namespace EcoTrip.Models
{
    public class EcoTripDbContext : DbContext
    {
        public EcoTripDbContext() { }

        public EcoTripDbContext(DbContextOptions options) : base(options){}
        public DbSet<Trips> trips { get; set; }
        public DbSet<Country> countrys { get; set; }
        public DbSet<TripsImage> trips_images { get; set; }
        public DbSet<TripsMapLocations> TripsMapLocations { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Users> Users => Set<Users>();
        public DbSet<Booking> Bookings { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=ecotrip;user=root;password=");
        }

    }
}
