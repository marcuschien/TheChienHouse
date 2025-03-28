using Microsoft.EntityFrameworkCore;
using LightspeedTakeHome.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<RetailContext>(options =>
    options.UseInMemoryDatabase("ProductList"));
builder.Services.AddDbContext<RetailContext>(options =>
    options.UseInMemoryDatabase("SaleList"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
