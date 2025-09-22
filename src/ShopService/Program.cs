using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ShopService.Data;
using ShopService.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetValue<string>("POSTGRES_CONNECTION")
                       ?? "Host=postgres-shop;Port=5432;Database=shopdb;Username=shopuser;Password=shoppass";

// Add DbContext
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add repository and controllers
builder.Services.AddScoped<ShopRepository>();
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Shop Service API",
        Version = "v1",
        Description = "API for managing in-game shop items"
    });
});

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    db.Database.Migrate();
}

// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop Service API v1");
        c.RoutePrefix = string.Empty;
    });
}

// Map controllers
app.MapControllers();

app.Run();
