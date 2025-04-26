using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RPI_API.Data;
using RPI_API.Models;
using RPI_API.Utils;

namespace RPI_API.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly DisplayContext _context;
        private readonly Emitter _emitter;

        public ImageController(DisplayContext context, Emitter emitter)
        {
            _context = context;
            _emitter = emitter;
        }

        // GET: api/image
        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            var images = await _context.ImageData.ToListAsync();

            return Ok(new { success = true, images = images });
        }

        // POST: api/image
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "No File Uploaded. "});

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest(new { success = false, message = "Invalid File Type."});

            var uploadsPath = Path.Combine("uploads");
            Directory.CreateDirectory(uploadsPath);

            var filePath = Path.Combine(uploadsPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"https://rpi-display.duckdns.org:3000/uploads/{file.FileName}";

            var imageObject = new ImageData
            {
                FileName = file.FileName,
                Url =  fileUrl
            };

            _context.ImageData.Add(imageObject);
            await _context.SaveChangesAsync();

            await _emitter.EmitAsync($"Image {fileUrl}", "display.set");

            return Ok(new { success = true, message = "File Uploaded Successfully"});
        }

        //POST: api/image/set
        [HttpPost("set")]
        public async Task<IActionResult> SetDisplayImage([FromBody] JObject jsondata)
        {
            var idString = jsondata["id"]?.ToString();

            if (idString == null) return BadRequest(new { success = false, message = "ID not provided." });

            int id = Int32.Parse(idString);

            var image = await _context.ImageData.FindAsync(id);
            if (image == null)
                return NotFound(new { success = false, message = "Image not found." });

            await _emitter.EmitAsync($"Image {image.Url}", "display.set");

            return Ok(new { success = true, message = "Image sent to display." });
        }

        // DELETE: api/image/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var image = await _context.ImageData.FindAsync(id);
            
            if (image == null)
                return NotFound(new { success = false, message = "Image not found." });

            // Remove the file from the file system
            var filePath = Path.Combine("uploads", image.FileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Remove from database
            _context.ImageData.Remove(image);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Image deleted successfully." });
        }
    }
}