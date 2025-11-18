using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assignment3_EquipmentRental_API_Group12.Models;
using Assignment3_EquipmentRental_API_Group12.UnitOfWork;

namespace Assignment3_EquipmentRental_API_Group12.Controllers
{
    /** Customer Management Endpoints
     * GET /api/customers - List all customers (Index page)
     * GET /api/customers/{id} - Get customer details (Details page)
     * POST /api/customers - Create customer (form)
     * PUT /api/customers/{id} - Update customer (Edit form)
     * DELETE /api/customers/{id} - Delete customer (Index)
     * GET /api/customers/{id}/rentals - Get customer rental history (My Rentals)
     * GET /api/customers/{id}/activate-rental - Check active rental (Dashboard)
     */

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;   // instance of IUnitOfWork

        // Constructor
        public CustomerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET /api/customers - List all customers (Index page)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<IEnumerable<CustomerController>> GetCustomers()
        {
            var customers = _unitOfWork.Customers.GetAll();
            return Ok(customers);   // return 200 OK with the list of customers
        }

        // GET /api/customers/{id} - Get customer details (Details page)
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult GetCustomerById(int i)
        {
            var customer = _unitOfWork.Customers.GetById(i);

            // if customer does not exist
            if (customer == null)
            {
                return NotFound(); // return 404 Not Found
            }
            return Ok(customer); // return 200 OK with the customer details
        }

        // POST /api/customers - Create customer (form)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Customer> CreateCustomer([FromBody] Customer customer)
        {
            // if request body, which is the customer object is null,
            if (customer == null)
            {
                return BadRequest(); // return 400 Bad Request
            }
            _unitOfWork.Customers.Add(customer);
            _unitOfWork.Complete(); // save changes to the database
            // return 201 Created
            return CreatedAtAction(nameof(GetCustomers), new { id = customer.Id }, customer);
        }

        // PUT /api/customers/{id} - Update customer (Edit form)
        [Authorize]
        [HttpPut("{id}")]
        public ActionResult<Customer> UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            var customer = _unitOfWork.Customers.GetById(id);

            // if customer does not exist (no customer with the given id)
            if (customer == null)
            {
                return NotFound(); // return 404 Not Found
            }

            // otherwise,
            // update customer properties
            customer.Role = updatedCustomer.Role;

            customer.UserName = updatedCustomer.UserName;
            customer.Password = updatedCustomer.Password;
            
            _unitOfWork.Customers.Update(customer);
            _unitOfWork.Complete(); // save changes to the database
            return Ok(customer); // return 200 OK with the updated customer
        }
    }
}
