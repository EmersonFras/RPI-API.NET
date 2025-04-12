using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace RPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _clientId;
        private readonly string _redirectUri;

        public AuthController()
        {
            _clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            _redirectUri = Environment.GetEnvironmentVariable("REDIRECT_URI");
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            string scope = "user-read-private user-read-email";
            // Query builder for params
            var query = HttpUtility.ParseQueryString(string.Empty);
            Console.WriteLine(_clientId);
            Console.WriteLine(_redirectUri);

            query["response_type"] = "code";
            query["scope"] = scope;
            query["client_id"] = _clientId;
            query["redirect_uri"] = _redirectUri;

            
            return Redirect("https://accounts.spotify.com/authorize?" + query.ToString());
        }
    }
}