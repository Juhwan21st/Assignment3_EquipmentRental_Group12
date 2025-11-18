using Assignment3_EquipmentRental_API_Group12.Data;
using Assignment3_EquipmentRental_API_Group12.Repositories;
using Assignment3_EquipmentRental_API_Group12.Repositories.Interfaces;
using Assignment3_EquipmentRental_API_Group12.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ----------------------------------
// Configure Entity Framework with In-Memory Database
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("EquipmentRentalDb"));

// Register Repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();

// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add JWT Authentication
builder.Services.AddAuthentication("Bearer")
	.AddJwtBearer("Bearer", options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("YourSuperSecretKeyHere1234567890")),
			ClockSkew = TimeSpan.Zero
		};
	});

// Add Authorization
builder.Services.AddAuthorization();
// ----------------------------------

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ----------------------------------
// Ensure the database is created when the application starts
using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	dbContext.Database.EnsureCreated();
}
// ----------------------------------

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
