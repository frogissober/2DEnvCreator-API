using _2DEnvCreator_API.Repositories;
using _2DEnvCreator_API.Interfaces;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IAuthenticationService, AspNetIdentityAuthenticationService>();

var sqlConnectionString = builder.Configuration["SqlConnectionString"];
var sqlConnectionStringFound = !string.IsNullOrEmpty(sqlConnectionString);

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 10;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddRoles<IdentityRole>()
.AddDapperStores(options =>
{
    options.ConnectionString = sqlConnectionString;
});

builder.Services.AddTransient<IEnvironmentRepository, EnvironmentRepository>(o => new EnvironmentRepository(sqlConnectionString));
builder.Services.AddTransient<IObject2DRepository, Object2DRepository>(o => new Object2DRepository(sqlConnectionString));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => $"The API is up. Connection string found: {(sqlConnectionStringFound ? "✅" : "❌")}");

app.MapGroup("/account").MapIdentityApi<IdentityUser>();

app.MapControllers().RequireAuthorization();

app.Run();
