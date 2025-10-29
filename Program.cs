using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using TheChienHouse.Middleware;
using TheChienHouse.Models;
using TheChienHouse.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the DI container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://192.168.4.24:3000")  // Frontend URL
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();
builder.Services.AddDbContext<RetailContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))
    )
);
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IEventFormService, EventFormService>();
builder.Services.AddScoped<IContactFormService, ContactFormService>();
// Add more services here as needed

var app = builder.Build();

// Enable CORS for the frontend
app.UseCors("AllowFrontend");

// Register global exception middleware early in the pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
