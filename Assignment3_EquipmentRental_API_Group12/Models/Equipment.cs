namespace Assignment3_EquipmentRental_API_Group12.Models
{
    // Equipmen Category can be "Heavy Machinery", "Power Tools", "Vehicles", "Safety", "Surveying"
    // Condition can be "New", "Excellent", "Good", "Fair", "Poor"
    // Buisiness Rules:
    // Equipment can olny be issued if available. When returned, IsAvailable = true.
    // Users can only have one active rental at a time.
    // Admins can override or cancel rentals.

    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Condition { get; set; }
        public decimal RentalPrice { get; set; }
        public bool IsAvailable { get; set; }

        // timestamp for when the equipment was added to the system
        public DateTime CreatedAt { get; set; }
    }
}
