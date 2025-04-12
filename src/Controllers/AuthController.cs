using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using RPI_API.Utils;

namespace RPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        public AuthController()
        {
            _clientId = Environment.GetEnvironmentVariable("CLIENT_ID")!;
            _clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET")!;
            _redirectUri = Environment.GetEnvironmentVariable("REDIRECT_URI")!;
        }

        // GET: api/auth/check
        [Authorize]
        [HttpGet("check")]
        public ActionResult Check()
        {
            return Ok( new { success = true, isAuthenticated = true, message = "Logged In" });
        }

        //GET: api/auth/refresh
        [Authorize]
        [HttpGet("refresh")]
        public async Task<ActionResult> Refresh()
        {
            using (var client = new HttpClient())
            {
                var refresh_token = User.Claims.FirstOrDefault(c => c.Type == "spotify_refresh_token")?.Value;

                if (refresh_token == null)
                {
                    return BadRequest(new { sucess = false });
                }

                // Body for the request
                var requestData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refresh_token)
                });

                var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

                var response = await client.PostAsync("https://accounts.spotify.com/api/token", requestData);

                // Reads the response
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, content);
                }

                //  Takes HTTP Response -> json
                var json = JsonDocument.Parse(content);
                
                // Extracts tokens from json
                var accessToken = json.RootElement.GetProperty("access_token").GetString();

                if (accessToken == null)
                {
                    return BadRequest(new { success = false });
                }

                // Util method to generate token with the claims for tokens.
                var jwt = JwtToken.GenerateJwtToken(accessToken, refresh_token);

                // Adds the jwt token as a cookie
                Response.Cookies.Append("jwt_token", jwt, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                // Return response including cookies
                return Ok(new { success = true, content });
            }
        }

        // GET: api/auth/login
        [HttpGet("login")]
        public IActionResult Login()
        {
            string scope = "user-read-private user-read-email";
            // Query builder for params
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["response_type"] = "code";
            query["scope"] = scope;
            query["client_id"] = _clientId;
            query["redirect_uri"] = _redirectUri;

            
            return Redirect("https://accounts.spotify.com/authorize?" + query.ToString());
        }

        // GET: api/auth/callback
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("No authroization code provided");
            }

            // Creates a client to make request through
            using (var client = new HttpClient())
            {

                // Body for the request
                var requestData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", _redirectUri),
                });

                // Encodes auth header string
                var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

                // Sets auth header to the one we made
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

                // Makes request to get access and refresh token
                var response = await client.PostAsync("https://accounts.spotify.com/api/token", requestData);

                // Reads the response
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, content);
                }


                //  Takes HTTP Response -> json
                var json = JsonDocument.Parse(content);
                
                // Extracts tokens from json
                var accessToken = json.RootElement.GetProperty("access_token").GetString();
                var refreshToken = json.RootElement.GetProperty("refresh_token").GetString();

                if (accessToken == null || refreshToken == null)
                {
                    return BadRequest(new { success = false });
                }

                // Util method to generate token with the claims for tokens.
                var jwt = JwtToken.GenerateJwtToken(accessToken, refreshToken);

                // Adds the jwt token as a cookie
                Response.Cookies.Append("jwt_token", jwt, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                // Return response including cookies
                return Ok(new { success = true });
            }
        }

        //POST: api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("jwt_token", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1)
            });

            return Ok(new { success = true });
        }
    }
}