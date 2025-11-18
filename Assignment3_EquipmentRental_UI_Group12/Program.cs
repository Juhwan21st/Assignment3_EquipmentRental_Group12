using Assignment3_EquipmentRental_UI_Group12.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

// Add MVC (Razor Views)
builder.Services.AddControllersWithViews();


// Google login issues an auth cookie for the UI; UI then mints its own JWT when calling the API
builder.Services.AddAuthentication(opt =>
{
	// ---------- Assignment 3 ----------
	// <A3_Instruction>: Add Google OpenID Connect in Program.cs using Cookie + OIDC schemes.
	opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;   // use cookie for UI session
	opt.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;        // challenge via Google
})
.AddCookie(o =>
{
	o.LoginPath = "/auth/login";
	o.LogoutPath = "/auth/logout";
	o.AccessDeniedPath = "/auth/denied";
})
.AddGoogle(o =>
{
	// Register a Web application OAuth client in Google Cloud Console,
	// and set the Authorized redirect URI to: https://localhost:{UI_PORT}/signin-google
	o.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
	o.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;

	// Ensure we get email (needed for role mapping) and basic profile
	o.Scope.Add("email");
	o.Scope.Add("profile");
	o.Scope.Add("openid");
});

// This transformer runs AFTER Google login, BEFORE authorization,
// and adds ClaimTypes.Role ("Admin" or "User") based on email list in appsettings.
builder.Services.AddScoped<IClaimsTransformation, RoleClaimsTransformer>();

builder.Services.AddAuthorization();

// JWT minting service (creates short-lived JWTs that include the user's role claims)
builder.Services.AddScoped<JwtService>();

builder.Services.AddHttpClient<ApiClient>(c =>
{
	// Points to API; configurable in appsettings
	c.BaseAddress = new Uri(builder.Configuration["Api:BaseAddress"]!);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();   // reads/writes the auth cookie
app.UseAuthorization();    // evaluates [Authorize] and roles

// Conventional Routing
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the app
app.Run();
