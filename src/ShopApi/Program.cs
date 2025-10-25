using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Repos;
using ShopApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Kestrel
builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(8085));

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<ShopDbContext>(o => o.UseNpgsql(connectionString));

// Redis
var redisConfig = builder.Configuration["Redis:Configuration"] ?? builder.Configuration["REDIS__CONFIG"] ?? "localhost:6379";
builder.Services.AddStackExchangeRedisCache(o => o.Configuration = redisConfig);

// Character Client
var characterUrl = builder.Configuration["CharacterService:Url"] ?? builder.Configuration["CHARACTER_SERVICE_URL"] ?? "http://localhost:4002";
builder.Services.AddHttpClient<ICharacterClient, CharacterClient>(c =>
{
    c.BaseAddress = new Uri(characterUrl);
    c.Timeout = TimeSpan.FromSeconds(5);
});

// DI
builder.Services.AddScoped<ItemRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<ServiceRegistration>();

var app = builder.Build();

// Ensure DB with retry loop
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();

    var retries = 10; // Try 10 times
    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            Console.WriteLine("Database connected and migrations applied.");
            break;
        }
        catch
        {
            retries--;
            Console.WriteLine("Waiting for database... retrying in 5 seconds");
            await Task.Delay(5000);
        }
    }

    if (retries == 0) throw new Exception("Failed to connect to database after multiple attempts.");
}

// Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

// Health Check
app.MapGet("/health", () => Results.Json(new { status = "healthy" }));

app.Run();
