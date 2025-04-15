using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private readonly DisplayContext _context;

        public DisplayController(Emitter emitter, DisplayContext context)
        {
            _emitter = emitter;
            _context = context;
        }

        // Set/Update the current Display
        // POST api/display
        [HttpPost]
        public async Task<IActionResult> UpdateDisplay([FromBody] JObject jsondata)
        {
            var data = jsondata["display"]?.ToString();

            if (data != null)
                {
                    bool ack = await _emitter.EmitAsync(data, "display.set");

                    if (ack) return Ok(new { success = true });
                    else return BadRequest("Failed to send message to Display Service");
                }
                else
                {
                    return BadRequest("Invalid JSON format: 'data' field is missing.");
                }
        }

        // Clearing the display
        // POST api/display/clear
        [HttpPost("clear")]
        public async Task<IActionResult> ClearDisplay()
        {
            bool ack = await _emitter.EmitAsync("clear", "display.set");

            if (ack) return Ok(new { success = true });
            else return BadRequest("Failed to send message to Display Service");
        }

        // Updates the display data, text, on/off time etc.
        // POST api/display/data
        [HttpPost("data")]
        public async Task<IActionResult> UpdateDisplayData([FromBody] DisplayDataDto data)
        {
            if (data == null)
            {
                return BadRequest("Invalid data");
            }

            Console.WriteLine($"[Display Data] Sending message {data.ToString()}");

            var json = JsonConvert.SerializeObject(data);
            // Waits for publisher confirmation 
            bool ack = await _emitter.EmitAsync(message: json,
                                                "display.data");


            /*
            * TO-DO: This ACKS before knowing if the data in the exchange was distributed
            * to the subscribers. This is not a problem for now, but it would be
            * better to wait for the confirmation from the display service before updating
            * the database.
            */
            if (ack)
            {
                var weatherEntry = await _context.WeatherDisplayData.FirstOrDefaultAsync();
                var displayEntry = await _context.DisplayData.FirstOrDefaultAsync();
                if (weatherEntry != null && displayEntry != null)
                {
                    if (data.Text != null)
                        weatherEntry.Text = data.Text;

                    if (data.StartTime != null)
                        displayEntry.StartTime = data.StartTime;

                    if (data.StopTime != null)
                        displayEntry.StopTime = data.StopTime;

                    displayEntry.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine($"[Display] No entry found in the database");
                }

                return Ok(new { success = true });
            }
            else return BadRequest("Failed to send message to Display Service");
        }
    }
}
