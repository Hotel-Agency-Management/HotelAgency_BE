using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Booking.Configurations;
using Booking.Constants;
using Booking.Models;
using Microsoft.IdentityModel.Tokens;

namespace Booking.Utils
{
    public static class JwtUtils
    {
        public static string GenerateToken(ApplicationUser user, string role, JwtSettings jwtSettings)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimConstant.FIRST_NAME, user.FirstName ?? string.Empty),
                new Claim(ClaimConstant.LAST_NAME, user.LastName ?? string.Empty),
                new Claim(ClaimTypes.Role, role),
            };

            if (!string.IsNullOrEmpty(user.UserName))
                claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName));

            if (user.AgencyId.HasValue)
                claims.Add(new Claim(ClaimConstant.AGENCY_Id, user.AgencyId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(jwtSettings.DurationInMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
