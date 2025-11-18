using Assignment3_EquipmentRental_API_Group12.Repositories.Interfaces;

namespace Assignment3_EquipmentRental_API_Group12.UnitOfWork
{
	/// <summary>
	///     IUnit Of Work Interface
	/// </summary>
	public interface IUnitOfWork
	{
		// defines properties for accessing repositories
		ICustomerRepository Customers { get; }
		IEquipmentRepository Equipments { get; }
		IRentalRepository Rentals { get; }

		// method to save all changes to the database
		int Complete();
	}
}
