using Assignment3_EquipmentRental_UI_Group12.Models;
using Assignment3_EquipmentRental_UI_Group12.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Assignment3_EquipmentRental_UI_Group12.Controllers
{
	public class HomeController : Controller
	{
		// API client for making requests to the backend
		private readonly ApiClient _apiClient;
		private readonly ILogger<HomeController> _logger;   //default logger from template

		// Constructor
		// Dependency Injection of logger and ApiClient
		public HomeController(ILogger<HomeController> logger, ApiClient apiClient)
		{
			_logger = logger;
			_apiClient = apiClient;
		}

		[AllowAnonymous]
		public IActionResult Index()
		{
			// if the user already logged in, show the dashboard instead
			if (User.Identity != null && User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Dashboard");
			}
			return View();
		}

		public IActionResult Dashboard()
		{
			if (User.IsInRole("Admin"))
			{
				return RedirectToAction("AdminDashboard");
			}
			else
			{
				return RedirectToAction("UserDashboard");
			}
		}

		// Admin Dashboard
		[Authorize(Roles = "Admin")]
		public IActionResult AdminDashboard()
		{
			ViewBag.UserName = User.Identity?.Name;
			ViewBag.Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
			ViewBag.Role = "Admin";

			return View();
		}

		// User Dashboard
		[Authorize(Roles = "User")]
		public IActionResult UserDashboard()
		{
			ViewBag.UserName = User.Identity?.Name;
			ViewBag.Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
			ViewBag.Role = "User";

			return View();
		}

		public IActionResult About()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
