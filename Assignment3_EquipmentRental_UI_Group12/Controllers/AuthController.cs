using Microsoft.AspNetCore.Mvc;

namespace Assignment3_EquipmentRental_UI_Group12.Controllers
{
	public class AuthController : Controller
	{
		[HttpGet("auth/login")]
		public IActionResult Login(string returnUrl = "/")
		{
			return Challenge(new Microsoft.AspNetCore.Authentication.AuthenticationProperties
			{
				RedirectUri = returnUrl
			}, "Google");
		}

		[HttpPost("auth/logout")]
		public IActionResult Logout()
		{
			return SignOut(new Microsoft.AspNetCore.Authentication.AuthenticationProperties
			{
				// move to home page after logout
				RedirectUri = Url.Action("Index", "Home")
			},
			Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
		}

		[HttpGet("auth/denied")]
		public IActionResult AccessDenied()
		{
			return Content("Access Denied");
		}
	}
}
