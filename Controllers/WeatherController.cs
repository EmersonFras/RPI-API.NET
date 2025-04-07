using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPI_API.Data;

namespace RPI_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {

        public readonly WeatherDisplayContext _context;

        public WeatherController(WeatherDisplayContext context)
        {
            _context = context;
        }

        // GET api/weather
        [HttpGet]
        public async Task<IActionResult> GetWeather()
        {
            var entry = await _context.WeatherDisplayData.FirstOrDefaultAsync();
            if (entry == null)
            {
                return NotFound("No weather data found");
            }
            return Ok(entry);
        }
    }
}
