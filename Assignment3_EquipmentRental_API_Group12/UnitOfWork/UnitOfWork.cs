using Assignment3_EquipmentRental_API_Group12.Data;
using Assignment3_EquipmentRental_API_Group12.Repositories.Interfaces;

namespace Assignment3_EquipmentRental_API_Group12.UnitOfWork
{
    /// <summary>
    ///     Unit Of Work implementation
    ///     Provides aceess to repositories
    ///     Implements Complete() method that saves all changes to the database
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context; // instance of AppDbContext

        // Properties for accessing repositories
        public ICustomerRepository Customers { get; set; }
        public IEquipmentRepository Equipments { get; set; }
        public IRentalRepository Rentals { get; set; }

        // Constructor that takes AppDbContext and repositories as parameters
        public UnitOfWork(AppDbContext context, ICustomerRepository customerRepository, IEquipmentRepository equipmentRepository, IRentalRepository rentalRepository)
        {
            // assign the context parameter to the _context field
            _context = context;

            // initialize repository properties
            Customers = customerRepository;
            Equipments = equipmentRepository;
            Rentals = rentalRepository;
        }

        // Save all changes made in the context to the database
        public int Complete()
        {
            // Call the SaveChanges() method of AppDbContext
            // through this, multiple operations performed across different repositories can be handled as a single transaction
            return _context.SaveChanges();
        }
    }
}
