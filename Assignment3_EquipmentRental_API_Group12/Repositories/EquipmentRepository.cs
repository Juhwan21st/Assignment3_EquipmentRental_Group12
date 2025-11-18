using Assignment3_EquipmentRental_API_Group12.Data;
using Assignment3_EquipmentRental_API_Group12.Models;
using Assignment3_EquipmentRental_API_Group12.Repositories.Interfaces;

namespace Assignment3_EquipmentRental_API_Group12.Repositories
{
    /// <summary>
    ///     Equipment Repository Implementation
    ///     inherits basic CRUD operations from Repository<Equipment>
    ///     and also implements IEquipmentRepository interface to use specific methods for Equipment entity
    /// </summary>
    public class EquipmentRepository : Repository<Equipment>, IEquipmentRepository
    {
        // Constructor
        // no need to implement anything here
        // initialized _context and _dbSet are inherited from the base class (Repository<T>)
        public EquipmentRepository(AppDbContext context) : base(context)
        {
        }
    }
}
