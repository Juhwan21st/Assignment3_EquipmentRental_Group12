using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Assignment3_EquipmentRental_UI_Group12.Services
{
	public class RoleClaimsTransformer : IClaimsTransformation
	{
		private readonly IConfiguration configuration;

		public RoleClaimsTransformer(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
		{
			var identity = principal.Identities.FirstOrDefault(i => i.IsAuthenticated);
			if (identity == null)
			{
				return Task.FromResult(principal);
			}

			if (identity.HasClaim(c => c.Type == ClaimTypes.Role))
			{
				return Task.FromResult(principal);
			}

			var email = principal.FindFirstValue(ClaimTypes.Email);

			var adminEmails = configuration
				.GetSection("AuthDemo:AdminEmails")
				.Get<List<string>>() ?? new List<string>();

			// Assign role based on email
			var role = adminEmails.Contains(email) ? "Admin" : "User";

			// add Role claim
			identity.AddClaim(new Claim(ClaimTypes.Role, role));

			return Task.FromResult(principal);
		}
	}
}
