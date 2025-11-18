using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assignment3_EquipmentRental_API_Group12.Models;
using Assignment3_EquipmentRental_API_Group12.UnitOfWork;
using System.Security.Claims;

namespace Assignment3_EquipmentRental_API_Group12.Controllers
{
    /// <summary>
    /// Rental Management Controller
    /// Handles equipment rental operations with business rules enforcement
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

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
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var rentals = _unitOfWork.Rentals.GetAll();

            // If user is not Admin, filter to show only their rentals
            if (userRole != "Admin")
            {
                var customer = _unitOfWork.Customers.GetAll()
                    .FirstOrDefault(c => c.UserName == userName);

                if (customer == null)
                {
                    return Unauthorized(new { message = "Customer not found." });
                }

                rentals = rentals.Where(r => r.CustomerId == customer.Id).ToList();
            }

            return Ok(rentals);
        }

        /// <summary>
        /// GET: api/rentals/{id}
        /// Get rental details
        /// Admin sees all, User sees only their own
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            var rental = _unitOfWork.Rentals.GetById(id);

            if (rental == null)
            {
                return NotFound(new { message = $"Rental with ID {id} not found." });
            }

            // Check if user has permission to view this rental
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole != "Admin")
            {
                var customer = _unitOfWork.Customers.GetAll()
                    .FirstOrDefault(c => c.UserName == userName);

                if (customer == null || rental.CustomerId != customer.Id)
                {
                    return Forbid();
                }
            }

            return Ok(rental);
        }

        /// <summary>
        /// POST: api/rentals/issue
        /// Issue equipment (Form)
        /// Business Rule: User can only have one active rental at a time
        /// </summary>
        [HttpPost("issue")]
        [Authorize]
        public IActionResult IssueEquipment([FromBody] RentalRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Rental data is required." });
            }

            // Get current user
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            int customerId;

            // If Admin, they can specify customer ID; otherwise use logged-in user
            if (userRole == "Admin" && request.CustomerId.HasValue)
            {
                customerId = request.CustomerId.Value;
            }
            else
            {
                var customer = _unitOfWork.Customers.GetAll()
                    .FirstOrDefault(c => c.UserName == userName);

                if (customer == null)
                {
                    return Unauthorized(new { message = "Customer not found." });
                }

                customerId = customer.Id;
            }

            // Validate customer exists
            var targetCustomer = _unitOfWork.Customers.GetById(customerId);
            if (targetCustomer == null)
            {
                return BadRequest(new { message = "Customer not found." });
            }

            // Business Rule: Check if user already has an active rental
            var activeRentals = _unitOfWork.Rentals.GetAll()
                .Where(r => r.CustomerId == customerId && r.ReturnedAt == null);

            if (activeRentals.Any())
            {
                return BadRequest(new { message = "Customer already has an active rental. Please return it before issuing a new one." });
            }

            // Validate equipment exists
            var equipment = _unitOfWork.Equipments.GetById(request.EquipmentId);
            if (equipment == null)
            {
                return BadRequest(new { message = "Equipment not found." });
            }

            // Business Rule: Equipment must be available
            if (!equipment.IsAvailable)
            {
                return BadRequest(new { message = "Equipment is not available for rental." });
            }

            // Create new rental
            var rental = new Rental
            {
                EquipmentId = request.EquipmentId,
                CustomerId = customerId,
                IssuedAt = DateTime.UtcNow,
                DueDate = request.DueDate ?? DateTime.UtcNow.AddDays(7), // Default 7 days
                ReturnedAt = null
            };

            _unitOfWork.Rentals.Add(rental);

            // Update equipment availability
            equipment.IsAvailable = false;
            _unitOfWork.Equipments.Update(equipment);

            _unitOfWork.Complete();

            return CreatedAtAction(nameof(GetById), new { id = rental.Id }, rental);
        }

        /// <summary>
        /// POST: api/rentals/return
        /// Return equipment (Form)
        /// Business Rule: Set IsAvailable = true when returned
        /// </summary>
        [HttpPost("return")]
        [Authorize]
        public IActionResult ReturnEquipment([FromBody] ReturnRequest request)
        {
            if (request == null || request.RentalId <= 0)
            {
                return BadRequest(new { message = "Rental ID is required." });
            }

            var rental = _unitOfWork.Rentals.GetById(request.RentalId);

            if (rental == null)
            {
                return NotFound(new { message = "Rental not found." });
            }

            // Check if already returned
            if (rental.ReturnedAt.HasValue)
            {
                return BadRequest(new { message = "Equipment has already been returned." });
            }

            // Check user permission (Users can only return their own rentals)
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole != "Admin")
            {
                var customer = _unitOfWork.Customers.GetAll()
                    .FirstOrDefault(c => c.UserName == userName);

                if (customer == null || rental.CustomerId != customer.Id)
                {
                    return Forbid();
                }
            }

            // Update rental
            rental.ReturnedAt = DateTime.UtcNow;
            _unitOfWork.Rentals.Update(rental);

            // Update equipment availability
            var equipment = _unitOfWork.Equipments.GetById(rental.EquipmentId);
            if (equipment != null)
            {
                equipment.IsAvailable = true;
                _unitOfWork.Equipments.Update(equipment);
            }

            _unitOfWork.Complete();

            return Ok(new { message = "Equipment returned successfully.", rental });
        }

        /// <summary>
        /// GET: api/rentals/active
        /// Get active rentals
        /// Admin sees all, User sees only their own
        /// </summary>
        [HttpGet("active")]
        [Authorize]
        public IActionResult GetActive()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var activeRentals = _unitOfWork.Rentals.GetAll()
                .Where(r => r.ReturnedAt == null);

            // If user is not Admin, filter to show only their rentals
            if (userRole != "Admin")
            {
                var customer = _unitOfWork.Customers.GetAll()
                    .FirstOrDefault(c => c.UserName == userName);

                if (customer == null)
                {
                    return Unauthorized(new { message = "Customer not found." });
                }

                activeRentals = activeRentals.Where(r => r.CustomerId == customer.Id);
            }

            return Ok(activeRentals.ToList());
        }

        /// <summary>
        /// GET: api/rentals/completed
        /// Get completed rentals
        /// Admin sees all, User sees only their own
        /// </summary>
        [HttpGet("completed")]
        [Authorize]
        public IActionResult GetCompleted()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var completedRentals = _unitOfWork.Rentals.GetAll()
                .Where(r => r.ReturnedAt != null);

            // If user is not Admin, filter to show only their rentals
            if (userRole != "Admin")
            {
                var customer = _unitOfWork.Customers.GetAll()
                    .FirstOrDefault(c => c.UserName == userName);

                if (customer == null)
                {
                    return Unauthorized(new { message = "Customer not found." });
                }

                completedRentals = completedRentals.Where(r => r.CustomerId == customer.Id);
            }

            return Ok(completedRentals.ToList());
        }

        /// <summary>
        /// GET: api/rentals/overdue
        /// Get overdue rentals (Admin only)
        /// Overdue = DueDate < DateTime.Now && ReturnedAt == null
        /// </summary>
        [HttpGet("overdue")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetOverdue()
        {
            var overdueRentals = _unitOfWork.Rentals.GetAll()
                .Where(r => r.ReturnedAt == null && r.DueDate < DateTime.UtcNow)
                .ToList();

            return Ok(overdueRentals);
        }

        /// <summary>
        /// GET: api/rentals/equipment/{equipmentId}
        /// Equipment rental history (Admin only)
        /// </summary>
        [HttpGet("equipment/{equipmentId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetEquipmentHistory(int equipmentId)
        {
            var equipment = _unitOfWork.Equipments.GetById(equipmentId);
            if (equipment == null)
            {
                return NotFound(new { message = "Equipment not found." });
            }

            var rentalHistory = _unitOfWork.Rentals.GetAll()
                .Where(r => r.EquipmentId == equipmentId)
                .OrderByDescending(r => r.IssuedAt)
                .ToList();

            return Ok(rentalHistory);
        }

        /// <summary>
        /// PUT: api/rentals/{id}
        /// Extend rental (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult ExtendRental(int id, [FromBody] ExtendRentalRequest request)
        {
            var rental = _unitOfWork.Rentals.GetById(id);

            if (rental == null)
            {
                return NotFound(new { message = "Rental not found." });
            }

            if (rental.ReturnedAt.HasValue)
            {
                return BadRequest(new { message = "Cannot extend a completed rental." });
            }
            //-----
            if (request == null || request.NewDueDate == null)
            {
                return BadRequest(new { message = "New due date is required." });
            }

            if (request.NewDueDate <= rental.DueDate)
            {
                return BadRequest(new { message = "New due date must be after the current due date." });
            }

            rental.DueDate = request.NewDueDate.Value;
            _unitOfWork.Rentals.Update(rental);
            _unitOfWork.Complete();

            return Ok(new { message = "Rental extended successfully.", rental });
        }
    }

    // Request models
    public class RentalRequest
    {
        public int EquipmentId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class ReturnRequest
    {
        public int RentalId { get; set; }
    }

    public class ExtendRentalRequest
    {
        public DateTime? NewDueDate { get; set; }
    }
    //------
}
