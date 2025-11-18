namespace Assignment3_EquipmentRental_API_Group12.Models
{
	// ---------- Assignment 3 ----------
	// <A3_Instruction>:Create an AppUser model and add it to your AppDbContext
	/**
	 * AppUser model to store Google OAuth login user information.
	 */
	public class AppUser
	{
		public int Id { get; set; }
		public string Email { get; set; } = default!;
		public string Role { get; set; } = "User"; // "Admin" or "User"
		public string? ExternalProvider { get; set; } = "Google";
		public string? ExternalId { get; set; }
	}
}