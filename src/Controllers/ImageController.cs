using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPI_API.Data;
using RPI_API.Models;

namespace RPI_API.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly DisplayContext _context;

        public ImageController(DisplayContext context)
        {
            _context = context;
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

            var fileUrl = $"/uploads/{file.FileName}";

            var imageObject = new ImageData
            {
                FileName = file.FileName,
                Url =  $"https://rpi-display.duckdns.org:3000/uploads/{file.FileName}"
            };

            _context.ImageData.Add(imageObject);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, url = fileUrl, message = "File Uploaded Successfully"});
        }
    }
}