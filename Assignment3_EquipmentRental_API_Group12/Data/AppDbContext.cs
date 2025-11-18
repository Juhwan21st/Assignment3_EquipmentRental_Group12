using Microsoft.EntityFrameworkCore;
using Assignment3_EquipmentRental_API_Group12.Models;

namespace Assignment3_EquipmentRental_API_Group12.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets for Equipment, Customer, and Rental
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Rental> Rentals { get; set; }

		// ---------- Assignment 3 ----------
		// <A3_Instruction>:Create an AppUser model and add it to your AppDbContext
		// DbSet for AppUser
		public DbSet<AppUser> AppUsers { get; set; }
		// ----------------------------------

		// Seed data
		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- Assignment 3 ----------
            // <A3_Instruction>:Seed at least one Admin user (email-based).
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser 
                { 
                    Id = 1, 
                    Email = "jhverse21st@gmail.com", 
                    Role = "Admin", 
                    ExternalProvider = "Google", 
                    ExternalId = null 
                },
                new AppUser
                {
                    Id = 2,
                    Email = "marvelous21st@gmail.com",
                    Role = "User",
                    ExternalProvider = "Google",
                    ExternalId = null
                });
			// ----------------------------------


			// Seed minimum 5 equipment items (different categories & conditions)
			// Equipmen Category can be "Heavy Machinery", "Power Tools", "Vehicles", "Safety", "Surveying"
			// Condition can be "New", "Excellent", "Good", "Fair", "Poor"
			// Buisiness Rules:
			// Equipment can olny be issued if available. When returned, IsAvailable = true.
			// Users can only have one active rental at a time.
			// Admins can override or cancel rentals.
			modelBuilder.Entity<Equipment>().HasData(
                new Equipment { Id = 1, Name = "Heavy Machine 1", Description = "Equipment for heavy work", Category = "Heavy Machinery", Condition = "New", RentalPrice = 111.11m, IsAvailable = false, CreatedAt = new DateTime(2025, 1, 1) },
                new Equipment { Id = 2, Name = "Power Tool 1", Description = "Tool for power work", Category = "Power Tools", Condition = "Excellent", RentalPrice = 222.22m, IsAvailable = true, CreatedAt = new DateTime(2025, 2, 2) },
                new Equipment { Id = 3, Name = "Vehicle 1", Description = "Vehicle for transportation", Category = "Vehicles", Condition = "Good", RentalPrice = 333.33m, IsAvailable = true, CreatedAt = new DateTime(2025, 3, 3) },
                new Equipment { Id = 4, Name = "Safety Equipment 1", Description = "Equipment for safety", Category = "Safety", Condition = "Fair", RentalPrice = 444.44m, IsAvailable = true, CreatedAt = new DateTime(2025, 4, 4) },
                new Equipment { Id = 5, Name = "Survey Tool 1", Description = "Tool for surveying", Category = "Surveying", Condition = "Poor", RentalPrice = 555.55m, IsAvailable = true, CreatedAt = new DateTime(2025, 5, 5) }
            );



            // Seed 1 Admin and 5 Users
            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, Name = "Juhwan Seo", Email = "admin@gmail.com", UserName = "admin", Password = "adminpw", Role = "Admin" },
                new Customer { Id = 2, Name = "Virginia Woolf", Email = "user1@gmail.com", UserName = "user1", Password = "user1pw", Role = "User" },
                new Customer { Id = 3, Name = "Bell Hooks", Email = "user2@gmail.com", UserName = "user2", Password = "user2pw", Role = "User" },
                new Customer { Id = 4, Name = "Chimamanda Ngozi Adichie", Email = "user3@gmail.com", UserName = "user3", Password = "user3pw", Role = "User" },
                new Customer { Id = 5, Name = "Matilda Joslyn Gage", Email = "user4@gmail.com", UserName = "user4", Password = "user4pw", Role = "User" },
                new Customer { Id = 6, Name = "Simone de Beauvoir", Email = "user5@gmail.com", UserName = "user5", Password = "user5pw", Role = "User" }
            );

            // Seed a few active and completed rentals for demonstration
            modelBuilder.Entity<Rental>().HasData(
                new Rental { Id = 1, EquipmentId = 1, CustomerId = 1, IssuedAt = new DateTime(2025, 10, 10), DueDate = new DateTime(2025, 10, 24), ReturnedAt = null },
                new Rental { Id = 2, EquipmentId = 2, CustomerId = 2, IssuedAt = new DateTime(2025, 09, 09), DueDate = new DateTime(2025, 09, 23), ReturnedAt = new DateTime(2025, 09, 30) }
            );
        }
    }
}
