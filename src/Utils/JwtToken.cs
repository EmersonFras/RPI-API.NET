using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace RPI_API.Utils
{
    public class JwtToken
    {
        private static readonly string _secretKey;

        static JwtToken()
        {
            _secretKey = Environment.GetEnvironmentVariable("JWT_KEY")!;   
        }

        public static string GenerateJwtToken(string access_token, string refresh_token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("spotify_access_token", access_token),
                new Claim("spotify_refresh_token", refresh_token)
            };

            var token = new JwtSecurityToken(
                issuer: "https://rpi-display.duckdns.org:3000",
                audience: "https://rpi-matrix.netlify.app",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}