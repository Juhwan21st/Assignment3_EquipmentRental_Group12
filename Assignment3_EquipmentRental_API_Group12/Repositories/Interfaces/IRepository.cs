namespace Assignment3_EquipmentRental_API_Group12.Repositories.Interfaces
{
	// Generic Repository pattern helps to abstract data access logic
	// that means, there is no need to decide which entity to work with from the start
	// can work with any entity type that mapped to database table
	// by using the type parameter "T"

	/// <summary>
	///     Generic Repository Interface
	///     Defines basic CRUD operations
	/// </summary>
	/// <typeparam name="T">
	///     "T" represents the entity type that the repository will manage
	///     It should be the one of the classes mapped to a database table
	///     (Customer, Equipment, Rental)
	/// </typeparam>
	public interface IRepository<T> where T : class
	{
		/* Basic CRUD operations */

		// GET all entities
		IEnumerable<T> GetAll();

		// GET entity by ID
		T GetById(int id);

		// ADD a new entity
		void Add(T entity);

		// UPDATE an entity
		void Update(T entity);

		// DELETE an entity
		void Delete(T entity);

	}
}
