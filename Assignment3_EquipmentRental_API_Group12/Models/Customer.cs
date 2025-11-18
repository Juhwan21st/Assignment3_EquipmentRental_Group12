namespace Assignment3_EquipmentRental_API_Group12.Models
{
	/**
     * Customer model to store customer information for equipment rental system.
     */
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Admin" or "User"
    }
}
