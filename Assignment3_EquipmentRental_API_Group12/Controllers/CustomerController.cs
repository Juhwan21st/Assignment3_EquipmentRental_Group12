using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assignment3_EquipmentRental_API_Group12.Models;
using Assignment3_EquipmentRental_API_Group12.UnitOfWork;

namespace Assignment3_EquipmentRental_API_Group12.Controllers
{
	/** Customer Management Endpoints (4 Marks)
     *  
     *  GET /api/customers - List all customers [Admin only]
     *  GET /api/customers/{id} - Get customer details [User: self]
     *  POST /api/customers - Create customer [Admin only]
     *  PUT /api/customers/{id} - Update customer [Admin: role] [User: self, Name/PW]
     *  DELETE /api/customers/{id} - Delete customer [Admin only]
     *  GET /api/customers/{id}/rentals - Get customer rental history [User: self]
     *  GET /api/customers/{id}/active-rental - Check active rental [User: self]
     */

	[Route("api/[controller]")]
	[ApiController]
	public class CustomerController : ControllerBase
	{
		// IUnitOfWork instance
		private readonly IUnitOfWork _unitOfWork;

		// Constructor with UnitOfWork injection
		public CustomerController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// GET: api/customers
		[Authorize(Roles = "Admin")]
		[HttpGet]
		public ActionResult<IEnumerable<Customer>> GetAllCustomers()
		{
			// Store the list of customers retrieved from the repository
			var customers = _unitOfWork.Customers.GetAll();

			// Return the list of customers and HTTP 200 OK status
			return Ok(customers);
		}

		// GET: api/customers/{id}
		[Authorize]
		[HttpGet("{id}")]
		public ActionResult GetCustomerById([FromRoute] int id)
		{
			var customer = _unitOfWork.Customers.GetById(id);

			if (customer == null)
			{
				// return HTTP 404 Not Found if the customer does not exist
				return NotFound();
			}

			// return the customer and HTTP 200 OK status
			return Ok(customer);
		}

		// POST: api/customers
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public ActionResult<Customer> CreateCustomer([FromBody] Customer customer)
		{
			if (customer == null)
			{
				// return HTTP 400 Bad Request if the customer is null
				return BadRequest();
			}

			_unitOfWork.Customers.Add(customer);
			_unitOfWork.Complete();

			// return HTTP 201 Created status
			return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
		}

		// PUT: api/customers/{id}
		[Authorize]
		[HttpPut("{id}")]
		public ActionResult<Customer> UpdateCustomer([FromRoute] int id, [FromBody] Customer customer)
		{
			var existingCustomer = _unitOfWork.Customers.GetById(id);

			if (existingCustomer == null)
			{
				// return HTTP 404 Not Found if the customer does not exist
				return NotFound();
			}

			// Update the existing customer's properties
			existingCustomer.Name = customer.Name;
			existingCustomer.Email = customer.Email;
			existingCustomer.UserName = customer.UserName;
			existingCustomer.Password = customer.Password;
			existingCustomer.Role = customer.Role;

			_unitOfWork.Customers.Update(existingCustomer);
			_unitOfWork.Complete();

			// return the updated customer and HTTP 200 OK status
			return Ok(existingCustomer);
		}
	}
}
