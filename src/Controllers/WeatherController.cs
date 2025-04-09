using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPI_API.Data;

namespace RPI_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {

        public readonly DisplayContext _context;

        public WeatherController(DisplayContext context)
        {
            _context = context;
        }

        // Gets all current weather display data
        // GET api/weather
        [HttpGet]
        public async Task<IActionResult> GetWeather()
        {
            var weatherEntry = await _context.WeatherDisplayData.FirstOrDefaultAsync();
            var displayEntry = await _context.DisplayData.FirstOrDefaultAsync();

            if (weatherEntry == null || displayEntry == null)
            {
                return NotFound("No weather data found");
            }

            // Needs to return { startTime, stopTime, Text }
            return Ok(new { success = true, startTime = displayEntry.StartTime, stopTime = displayEntry.StopTime, text = weatherEntry.Text });
        }
    }
}
