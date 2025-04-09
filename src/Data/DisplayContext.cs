using Microsoft.EntityFrameworkCore;
using RPI_API.Models;

namespace RPI_API.Data
{
    public class DisplayContext : DbContext
    {
        public DisplayContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<WeatherDisplayData> WeatherDisplayData { get; set; }
        public DbSet<DisplayData> DisplayData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Database seeding
            modelBuilder.Entity<WeatherDisplayData>().HasData(
                new WeatherDisplayData
                {
                    Id = 1,
                    Text = "",
                    Temperature = "72°F",
                    WeatherCode = "3",
                    UpdatedAt = new DateTime(2025, 4, 6, 8, 0, 0)
                }
            );

            ModelBuilder.Entity<DisplayData>().HasData(
                new DisplayData
                {
                    Id = 1,
                    StartTime = "08:00",
                    StopTime = "20:00",
                    UpdatedAt = new DateTime(2025, 4, 6, 8, 0, 0)
                }
            );
        }
    }
}
