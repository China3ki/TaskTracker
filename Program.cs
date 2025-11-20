using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TaskTracker.Entities;
using TaskTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// SQL version
var version = new MySqlServerVersion(new Version(10, 4, 32));

builder.Services.AddScoped<AuthService>();

builder.Services.AddDbContext<TasktrackerContext>(option =>
{
    option.UseMySql(builder.Configuration.GetConnectionString("Default"), version);
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
