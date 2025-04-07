using Microsoft.EntityFrameworkCore;
using RPI_API.Models;

namespace RPI_API.Data
{
    public class WeatherDisplayContext : DbContext
    {
        public WeatherDisplayContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<WeatherDisplayData> WeatherDisplayData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherDisplayData>().HasData(
                new WeatherDisplayData
                {
                    Id = 1,
                    Text = "",
                    StartTime = "08:00",
                    StopTime = "18:00",
                    Temperature = "72°F",
                    WeatherCode = "3",
                    UpdatedAt = new DateTime(2025, 4, 6, 8, 0, 0)
                }
            );
        }
    }
}
