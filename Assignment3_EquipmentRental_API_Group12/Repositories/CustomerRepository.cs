using Assignment3_EquipmentRental_API_Group12.Data;
using Assignment3_EquipmentRental_API_Group12.Models;
using Assignment3_EquipmentRental_API_Group12.Repositories.Interfaces;

namespace Assignment3_EquipmentRental_API_Group12.Repositories
{
    // Users(not admin) see only their own rentals
    // Users can only issue/return their own equimpment
    // Business Rules:
    //  - Each user can have only one active rental at a time.
    //  - Equipment status updates on issue/return.
    //  - Admins can cancel or force-return rentals.
    //  - Overdue = DueDate<DateTime.Now && ReturnedAt == null.

    /// <summary>
    ///     Customer Repository Implementation
    ///     inherits basic CRUD operations from Repository<Customer>
    ///     and also implements ICustomertRepository interface to use specific methods for Customer entity
    /// </summary>
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        // Constructor
        // no need to implement anything here
        // initialized _context and _dbSet are inherited from the base class (Repository<T>)
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }
    }
}
