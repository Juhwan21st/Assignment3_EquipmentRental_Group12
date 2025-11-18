using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assignment3_EquipmentRental_API_Group12.Models;
using Assignment3_EquipmentRental_API_Group12.UnitOfWork;
using System.Security.Claims;

namespace Assignment3_EquipmentRental_API_Group12.Controllers
{
	/** Rental Management Endpoints (6 Marks)
     *  
     *  GET /api/rentals - Get all rentals [User: self]
     *  GET /api/rentals/{id} - Get rental details [User: self]
     *  POST /api/rentals/issue - Issue equipment [User: self]
     *  POST /api/rentals/return - Return equipment [User: self]
     *  GET /api/rentals/active - Get active rentals [User: self]
     *  GET /api/rentals/completed - Get completed rentals [User: self]
     *  GET /api/rentals/overdue - Get overdue rentals [Admin only]
     *  GET /api/rentals/equipment/{equipmentId} - Equipment rental history
     *  PUT /api/rentals/{id} - Extend rental [Admin only]
     *  DELETE /api/rentals/{id} - Cancel rental [Admin only]
	 */

	/// <summary>
	/// Rental Management Controller
	/// Handles equipment rental operations with business rules enforcement
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class RentalController : ControllerBase
	{
		// IUnitOfWork instance
		private readonly IUnitOfWork _unitOfWork;

		// Constructor with UnitOfWork injection
		public RentalController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		/// <summary>
		/// GET: api/rentals
		/// Get all rentals (Index)
		/// Admin sees all, User sees only their own
		/// </summary>
		[HttpGet]
		[Authorize]
		public IActionResult GetAll()
		{
			var userRole = User.FindFirstValue(ClaimTypes.Role);
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			// Store the list of rentals retrieved from the repository
			var rentals = _unitOfWork.Rentals.GetAll();

			// If user is not Admin, filter to show only their rentals
			if (userRole != "Admin")
			{
				var customer = _unitOfWork.Customers.GetAll()
					.FirstOrDefault(c => c.Email == userEmail);

				if (customer == null)
				{
					// Return HTTP 404 Not Found if the customer does not exist
					return NotFound(new { message = "Unregistered Customer" });
				}

				rentals = rentals.Where(r => r.CustomerId == customer.Id).ToList();
			}

			// Return the list of rentals and HTTP 200 OK status
			return Ok(rentals);
		}

		// GET: api/rentals/{id}
		[HttpGet("{id}")]
		[Authorize]
		public ActionResult GetRentalById([FromRoute] int id)
		{
			var rental = _unitOfWork.Rentals.GetById(id);

			if (rental == null)
			{
				// return HTTP 404 Not Found if the rental does not exist
				return NotFound(new { message = $"Rental that has the given ID {id} doesn't exist" });
			}

			var userRole = User.FindFirstValue(ClaimTypes.Role);
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			if (userRole != "Admin")
			{
				var customer = _unitOfWork.Customers.GetAll()
					.FirstOrDefault(c => c.Email == userEmail);

				if (customer == null)
				{
					return NotFound(new { message = "Unregistered Customer" });
				}

				if (rental.CustomerId != customer.Id)
				{
					return Forbid();
				}
			}

			return Ok(rental);
		}

		// POST: api/rentals/issue

		// POST: api/rentals/return

		// GET: api/rentals/active
		[HttpGet("active")]
		[Authorize]
		public IActionResult GetActive()
		{
			var userRole = User.FindFirstValue(ClaimTypes.Role);
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			var activeRentals = _unitOfWork.Rentals.GetAll()
					.Where(r => r.ReturnedAt == null);

			// If user is not Admin, filter to show only their rentals
			if (userRole != "Admin")
			{
				var customer = _unitOfWork.Customers.GetAll()
						   .FirstOrDefault(c => c.Email == userEmail);

				if (customer == null)
				{
					return NotFound(new { message = "Unregistered Customer" });
				}

				activeRentals = activeRentals.Where(r => r.CustomerId == customer.Id);
			}

			return Ok(activeRentals.ToList());
		}

		// GET: api/rentals/completed
		[HttpGet("completed")]
		[Authorize]
		public IActionResult GetCompleted()
		{
			var userRole = User.FindFirstValue(ClaimTypes.Role);
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			var completedRentals = _unitOfWork.Rentals.GetAll()
				.Where(r => r.ReturnedAt != null);

			// If user is not Admin, filter to show only their rentals
			if (userRole != "Admin")
			{
				var customer = _unitOfWork.Customers.GetAll()
					.FirstOrDefault(c => c.Email == userEmail);

				if (customer == null)
				{
					// Return HTTP 404 Not Found if the customer does not exist
					return NotFound(new { message = "Unregistered Customer" });
				}

				completedRentals = completedRentals.Where(r => r.CustomerId == customer.Id);
			}

			return Ok(completedRentals.ToList());
		}

		// GET: api/rentals/overdue
		[HttpGet("overdue")]
		[Authorize(Roles = "Admin")]
		public IActionResult GetOverdue()
		{
			var overdueRentals = _unitOfWork.Rentals.GetAll()
				.Where(r => r.ReturnedAt == null && r.DueDate < DateTime.UtcNow)
				.ToList();

			return Ok(overdueRentals);
		}

		// GET: api/rentals/equipment/{equipmentId}

		// PUT: api/rentals/{id}
	}
}
