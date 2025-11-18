namespace Assignment3_EquipmentRental_API_Group12.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public int CustomerId { get; set; }

        // Timestamp for when the rental was issued
        public DateTime IssuedAt { get; set; }

        // Timestamp for when the rental is due
        public DateTime DueDate { get; set; }

        // Timestamp for when the rental was returned
        // nullable! it may not be returned yet at the time
        public DateTime? ReturnedAt { get; set; }
    }
}
