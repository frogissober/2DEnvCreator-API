using System.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var sqlConnectionString = builder.Configuration["SqlConnectionString"];
var sqlConnectionStringFound = !string.IsNullOrEmpty(sqlConnectionString);

// Adding the HTTP Context accessor to be injected. This is needed by the AspNetIdentityUserRepository
// to resolve the current user.
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();

// 
builder.Services.AddAuthorization();
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddDapperStores(options =>
    {
        options.ConnectionString = sqlConnectionString;
    });

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => $"The API is up . Connection string found: {(sqlConnectionStringFound ? "✅" : "❌")}");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapGroup("/account").MapIdentityApi<IdentityUser>();

app.MapControllers();

app.Run();
