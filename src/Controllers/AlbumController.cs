using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace RPI_API.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        // GET: api/album?search={value}
        [HttpGet]
        public async Task<IActionResult> GetAlbums([FromQuery] string search)
        {
            using (var client= new HttpClient())
            {
                var access_token = User.Claims.FirstOrDefault(c => c.Type == "spotify_access_token")?.Value;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

                var response = await client.GetAsync($"https://api.spotify.com/v1/search?q={search}&type=album&limit=10");

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, content);
                }

                //  Takes HTTP Response -> json
                var json = JsonDocument.Parse(content);
                
                Dictionary<string, string> albums = new Dictionary<string, string>();

                // Loop to pull out the image urls
                foreach (var item in json.RootElement.GetProperty("albums").GetProperty("items").EnumerateArray())
                {
                    if (item.TryGetProperty("images", out JsonElement imagesElement) && imagesElement.GetArrayLength() > 0)
                    {
                        var imageUrl = imagesElement[0].GetProperty("url").GetString();
                        var albumName = item.GetProperty("name").GetString();
                        if (imageUrl == null || albumName == null) {
                            return BadRequest(new { success = false }); 
                        }
                        albums[albumName] = imageUrl;
                    }
                }


                return Ok(new { success = true, items = albums });
            }
        }
    }
}