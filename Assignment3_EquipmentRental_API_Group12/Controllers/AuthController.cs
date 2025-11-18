using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Assignment3_EquipmentRental_API_Group12.Data;
using Assignment3_EquipmentRental_API_Group12.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Assignment3_EquipmentRental_API_Group12.Controllers
{
	/** Authentication Endpoints (4 Marks)
     *	Requirements:
     *  - Validate username and password from the database.
     *  - Generate JWT token with UserName and Role claims.
     *  - Apply [Authorize] and [Authorize(Roles = 'Admin')].
     *  - Redirect UI based on role (Admin → Admin Dashboard, User → User Dashboard).
     * Marks: JWT logic (2) + Role authorization (1) + UI integration (1)
     */

	/// <summary>
	///     API Controller for Authentication
	///     There are 2 types of customers: Admin and User
	///      POST /api/auth/login – Authenticate customer & return JWT token with role claims. 
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		// AppDbContext instance
		private readonly AppDbContext _context;

		// Constructor with AppDbContext injection
		public AuthController(AppDbContext context)
		{
			_context = context;
		}

		// the "FromBody" will read the username and password from the API request body
		// and bind them into the LoginRequest object (just like what we did in Week 4 Part 2 Lab)
		// url: https://localhost:7119/api/Auth/login
		// admin login: admin / adminpw
		// user login: user1 / user1pw
		[HttpPost("login")]
		public ActionResult<string> Login([FromBody] LoginRequest request)
		{
			// Validate user with data in the Customers table
			var customer = _context.Customers.FirstOrDefault(u => u.UserName == request.UserName && u.Password == request.Password);

			// if user not found, return Unauthorized
			if (customer == null)
			{
				return Unauthorized("Invalid username or password.");
			}
			// otherwise, generate JWT token
			var token = GenerateJwtToken(customer);
			return Ok(new { Token = token });
		}

		private object GenerateJwtToken(Customer customer)
		{
			// this claims variable represents the customer's information that will be included in the JWT token.
			var claims = new[]
			{
                // set the Name claim to the customer's UserName
                new Claim(ClaimTypes.Name, customer.UserName),
                // set the Role claim to the customer's Role (either "Admin" or "User")
                new Claim(ClaimTypes.Role, customer.Role)
			};

			// create new symmetric security key
			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("YourSuperSecretKeyHere1234567890"));
			// create new signing credentials using the "security key", and the "HMAC SHA256 algorithm"
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			// create the actual JWT token! 
			var token = new JwtSecurityToken(
				claims: claims, // set the claims with the variable "claims" that was defined above
				expires: DateTime.Now.AddMinutes(30),   // set token expiration time to 30 mins
				signingCredentials: creds); // set the signing credentials with the variable "creds" that was defined above
			return new JwtSecurityTokenHandler().WriteToken(token); // return the token to other methos, such as the "Login" method above
		}
	}
}
