using Microsoft.AspNetCore.Mvc;

namespace RPI_API.Controllers
{
    public class WeatherController : ControllerBase
    {
        public Task<IActionResult> GetWeather()
        {
            // Placeholder for actual weather data retrieval logic
            var weatherData = new
            {
                Temperature = "22°C",
                Condition = "Sunny"
            };


            return Task.FromResult<IActionResult>(Ok(weatherData));
        }
    }
}
