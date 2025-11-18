using Microsoft.EntityFrameworkCore;
using Assignment3_EquipmentRental_API_Group12.Data;
using Assignment3_EquipmentRental_API_Group12.Repositories.Interfaces;

namespace Assignment3_EquipmentRental_API_Group12.Repositories
{
    // Base Class for Repositories

    /// <summary>
    ///     Generic Repository Implementation (Inherits IRepository)
    ///     Provides basic CRUD operations
    ///     + Don't need to use SaveChanges() here, it will be handled by UnitOfWork!
    /// </summary>
    /// <typeparam name="T">
    ///     "T" represents the entity type that the repository will manage
    ///     It should be the one of the classes mapped to a database table
    ///     (Customer, Equipment, Rental)
    /// </typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context; // instance of AppDbContext
        protected DbSet<T> _dbSet;              // instance of DbSet for the entity type T


        // Constructor that takes AppDbContext as a parameter
        // and initializes the DbSet for the entity type T
        public Repository(AppDbContext context)
        {
            // assign the context parameter to the _context field
            _context = context;

            // initialize the _dbSet field using the Set<T>() method of AppDbContext,
            // which provides access to the DbSet for the entity type T
            _dbSet = _context.Set<T>(); 
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);   
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        
    }
}
