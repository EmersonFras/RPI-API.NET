using Microsoft.EntityFrameworkCore;
using RPI_API.Models;

namespace RPI_API.Data
{
    public class DisplayContext : DbContext
    {
        public DisplayContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<WeatherDisplayData> WeatherDisplayData { get; set; } = null!;
        public DbSet<DisplayData> DisplayData { get; set; } = null!;
        public DbSet<ImageData> ImageData { get; set; } = null!;

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

            modelBuilder.Entity<DisplayData>().HasData(
                new DisplayData
                {
                    Id = 1,
                    StartTime = "08:00",
                    StopTime = "20:00",
                    Brightness = "100",
                    UpdatedAt = new DateTime(2025, 4, 6, 8, 0, 0)
                }
            );
        }
    }
}
