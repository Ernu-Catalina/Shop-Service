using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Repos;
using ShopApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Kestrel
builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(80));

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

// Ensure DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    db.Database.Migrate();
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
app.MapGet("/health", () =>
{
    var ts = DateTime.UtcNow;
    Console.WriteLine($"[HEALTH] Health check requested at {ts:O}");
    return Results.Json(new { status = "healthy", service = "shop-service", timestamp = ts });
});

app.Run();
