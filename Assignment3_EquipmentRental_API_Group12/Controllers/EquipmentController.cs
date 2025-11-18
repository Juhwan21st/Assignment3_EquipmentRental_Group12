using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assignment3_EquipmentRental_API_Group12.Models;
using Assignment3_EquipmentRental_API_Group12.UnitOfWork;

namespace Assignment3_EquipmentRental_API_Group12.Controllers
{
	/** Equipment Management Endpoints (6 Marks)
     *  
     *  GET /api/equipment - Get all equipment (Index)
     *  GET /api/equipment/{id} - Get specific equipment (Details)
     *  POST /api/equipment - Add new equipment (Create) [Admin only]
     *  PUT /api/equipment/{id} - Update equipment (Edit) [Admin only]
     *  DELETE /api/equipment/{id} - Delete equipment [Admin only]
     *  GET /api/equipment/available - List available equipment
     *  GET /api/equipment/rented - Get rented equipment [Admin only]
     */

	/// <summary>
	/// Equipment Management Controller
	/// Handles all equipment CRUD operations with role-based authorization
	/// </summary>
	[Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
		// IUnitOfWork instance
		private readonly IUnitOfWork _unitOfWork;

		// Constructor with UnitOfWork injection
		public EquipmentController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET: api/equipment
		// url: https://localhost:7119/api/Equipment
		[Authorize]
		[HttpGet]
		public ActionResult<IEnumerable<Equipment>> GetAllEquipment()
		{
			// Store the list of equipment retrieved from the repository
			var equipment = _unitOfWork.Equipments.GetAll();

			// Return the list of equipment and HTTP 200 OK status
			return Ok(equipment);
		}

		// GET: api/equipment/{id}
		[Authorize]
		[HttpGet("{id}")]
		public ActionResult<Equipment> GetEquipmentById(int id)
		{
			var equipment = _unitOfWork.Equipments.GetById(id);

			if (equipment == null)
			{
				// return HTTP 404 Not Found if the equipment does not exist
				return NotFound(new { message = $"Equipment with ID {id} not found." });
			}

			// return the equipment and HTTP 200 OK status
			return Ok(equipment);
		}

		// POST: api/equipment

		// PUT: api/equipment/{id}

		// DELETE: api/equipment/{id}
		[Authorize(Roles = "Admin")]
		[HttpDelete("{id}")]
		public ActionResult DeleteEquipment([FromRoute] int id)
		{
			var existingEquipment = _unitOfWork.Equipments.GetById(id);

			if (existingEquipment == null)
			{
				// return HTTP 404 Not Found if the equipment does not exist
				return NotFound(new { message = $"Equipment that has the given {id} doesn't exist" });
			}

			_unitOfWork.Equipments.Delete(existingEquipment);
			_unitOfWork.Complete();

			// return HTTP 200 OK status
			return Ok(new { message = $"Equipment that has the given ID {id} has been deleted." });
		}

		// GET: api/equipment/available
		[Authorize]
		[HttpGet("available")]
		public ActionResult<IEnumerable<Equipment>> GetAvailableEquipment()
		{
			// Store available equipments retrieved from the repository
			var availableEquipment = _unitOfWork.Equipments.GetAll()
				.Where(e => e.IsAvailable == true);

			// return the list of available equipment with HTTP 200 OK status
			return Ok(availableEquipment);
		}

		// GET: api/equipment/rented
		[Authorize(Roles = "Admin")]
		[HttpGet("rented")]
		public ActionResult<IEnumerable<Equipment>> GetRentedEquipment()
		{
			// Store rented equipment retrieved from the repository
			var rentedEquipment = _unitOfWork.Equipments.GetAll()
				.Where(e => e.IsAvailable == false);

			// return the list of rented equipment and HTTP 200 OK status
			return Ok(rentedEquipment);
		}
	}
}
