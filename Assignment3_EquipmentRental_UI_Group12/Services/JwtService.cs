using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Assignment3_EquipmentRental_UI_Group12.Services
{
	/// <summary>
	///		JWT Token Generation Service
	///		Generates JWT tokens based on user information obtained from Google OAuth login
	/// </summary>
	public class JwtService
	{
		private readonly IConfiguration _configuration;
		public JwtService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateToken(ClaimsPrincipal user, TimeSpan? lifetime = null)
		{
			var issuer = _configuration["Jwt:Issuer"];
			var audience = _configuration["Jwt:Audience"];
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var now = DateTime.UtcNow;
			var claims = new List<Claim>
			{
				new (ClaimTypes.NameIdentifier, user.FindFirstValue(ClaimTypes.NameIdentifier)),
				new (ClaimTypes.Name, user.Identity.Name),
				new (ClaimTypes.Email, user.FindFirstValue(ClaimTypes.Email))
			};

			// Add role claims
			foreach (var role in user.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct())
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			// Create JWT token
			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				notBefore: now,
				expires: now.Add(lifetime ?? TimeSpan.FromHours(1)),    // 1 hr of default lifetime
				signingCredentials: creds
			);

			// Return the serialized JWT token as a string
			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
