using Microsoft.EntityFrameworkCore;

namespace EcoTrip.Models
{
    public class EcoTripDbContext : DbContext
    {
        public EcoTripDbContext() { }

        public EcoTripDbContext(DbContextOptions options) : base(options){}
        public DbSet<EcoTrips> eco_trips {  get; set; }
        public DbSet<Trips> trips { get; set; }
        public DbSet<Country> countrys { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=ecotrip;user=root;password=");
        }

    }
}
