using Microsoft.AspNetCore.Mvc;
using RPI_API.Utils;


namespace RPI_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisplayController : ControllerBase
    {
        private readonly Emitter _emitter;

        public DisplayController(Emitter emitter)
        {
            _emitter = emitter;
        }

        // POST api/display
        [HttpPost]
        public async Task<IActionResult> UpdateDisplay([FromBody] string data)
        {
            
            await _emitter.EmitAsync(data, "display.set");

            return Ok();
        }
    }
}
