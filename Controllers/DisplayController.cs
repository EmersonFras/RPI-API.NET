using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPI_API.Data;
using RPI_API.DTOs;
using RPI_API.Utils;


namespace RPI_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisplayController : ControllerBase
    {
        private readonly Emitter _emitter;
        private readonly WeatherDisplayContext _context;

        public DisplayController(Emitter emitter, WeatherDisplayContext context)
        {
            _emitter = emitter;
            _context = context;
        }

        // POST api/display
        [HttpPost]
        public async Task<IActionResult> UpdateDisplay([FromBody] string data)
        {
            
            bool ack = await _emitter.EmitAsync(data, "display.set");

            if (ack) return Ok();
            else return BadRequest("Failed to send message to Display Service");
        }

        // POST api/display/clear
        [HttpPost("clear")]
        public async Task<IActionResult> ClearDisplay()
        {
            bool ack = await _emitter.EmitAsync("clear", "display.set");

            if (ack) return Ok();
            else return BadRequest("Failed to send message to Display Service");
        }

        // POST api/display/data
        [HttpPost("data")]
        public async Task<IActionResult> UpdateDisplayData([FromBody] DisplayDataDto data)
        {
            if (data == null)
            {
                return BadRequest("Invalid data");
            }

            bool ack = await _emitter.EmitAsync(message: data.ToString(),
                                                "display.data");

            if (ack)
            {
                var entry = await _context.WeatherDisplayData.FirstOrDefaultAsync();
                if (entry != null)
                {
                    entry.Text = data.Text;
                    entry.StartTime = data.StartTime;
                    entry.StopTime = data.StopTime;

                    entry.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine($"[Display] No entry found in the database");
                }

                return Ok();
            }
            else return BadRequest("Failed to send message to Display Service");
        }
    }
}
