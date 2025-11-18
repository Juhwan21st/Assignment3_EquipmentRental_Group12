using System.Net.Http.Headers;
using System.Security.Claims;

namespace Assignment3_EquipmentRental_UI_Group12.Services
{
	/// <summary>
	///		API Client Service
	/// </summary>
	public class ApiClient
	{
		private readonly HttpClient _httpClient;
		private readonly JwtService _jwtService;

		public ApiClient(HttpClient httpClient, JwtService jwtService)
		{
			_httpClient = httpClient;
			_jwtService = jwtService;
		}

		private void AttachToken(ClaimsPrincipal user)
		{
			var token = _jwtService.GenerateToken(user);
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}

		// GET
		public async Task<string> GetProtectedDataAsync(string path, ClaimsPrincipal user)
		{
			// attach JWT token to header
			AttachToken(user);
			return await _httpClient.GetStringAsync(path);
		}

		// Other methods (POST, PUT, DELETE) can be added here similarly
	}
}
