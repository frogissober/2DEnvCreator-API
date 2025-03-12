using _2DEnvCreator_API.Repositories;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor(); // Register HTTP Context Accessor
builder.Services.AddControllers();

// Register the authentication service as scoped.
builder.Services.AddScoped<IAuthenticationService, AspNetIdentityAuthenticationService>();

var sqlConnectionString = builder.Configuration["SqlConnectionString"];
var sqlConnectionStringFound = !string.IsNullOrEmpty(sqlConnectionString);

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 10;
})
.AddRoles<IdentityRole>() // Adding role support if needed
.AddDapperStores(options =>
{
    options.ConnectionString = sqlConnectionString;
});

// Register your repository.
builder.Services.AddTransient<IEnvironmentRepository, EnvironmentRepository>(o => new EnvironmentRepository(sqlConnectionString));

var app = builder.Build();

app.UseHttpsRedirection();

// Ensure you call authentication before authorization.
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => $"The API is up. Connection string found: {(sqlConnectionStringFound ? "✅" : "❌")}");

app.MapGroup("/account").MapIdentityApi<IdentityUser>();

app.MapControllers().RequireAuthorization();

app.Run();
