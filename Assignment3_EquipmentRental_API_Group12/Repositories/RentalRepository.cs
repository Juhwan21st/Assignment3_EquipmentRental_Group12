using Assignment3_EquipmentRental_API_Group12.Data;
using Assignment3_EquipmentRental_API_Group12.Models;
using Assignment3_EquipmentRental_API_Group12.Repositories.Interfaces;

namespace Assignment3_EquipmentRental_API_Group12.Repositories
{
    /// <summary>
    ///     Rental Repository Implementation
    ///     inherits basic CRUD operations from Repository<Rental>
    ///     and also implements IRentalRepository interface to use specific methods for Rental entity
    /// </summary>
    public class RentalRepository : Repository<Rental>, IRentalRepository
    {
        // Constructor
        // no need to implement anything here
        // initialized _context and _dbSet are inherited from the base class (Repository<T>)
        public RentalRepository(AppDbContext context) : base(context)
        {
        }
    }
}
