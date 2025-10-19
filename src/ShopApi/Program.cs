using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Repos;
using ShopApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------- Configuration ----------------------
var configuration = builder.Configuration;

// ---------------------- Kestrel (listen on port 80) ----------------------
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // container port
});

// ---------------------- Database ----------------------
var connectionString = configuration.GetConnectionString("DefaultConnection")
                       ?? configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseNpgsql(connectionString));

// ---------------------- Redis Cache ----------------------
var redisConfig = configuration["Redis:Configuration"] ?? configuration["REDIS__CONFIG"] ?? "localhost:6379";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConfig;
});

// ---------------------- Character Service HTTP Client ----------------------
var characterUrl = configuration["CharacterService:Url"] ?? configuration["CHARACTER_SERVICE_URL"] ?? "http://localhost:4002";
builder.Services.AddHttpClient<ICharacterClient, CharacterClient>(client =>
{
    client.BaseAddress = new Uri(characterUrl);
    client.Timeout = TimeSpan.FromSeconds(5);
});

// ---------------------- Dependency Injection ----------------------
builder.Services.AddScoped<ItemRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---------------------- Ensure DB & Seed ----------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    db.Database.Migrate(); // this will apply migrations and insert seed data
}


// ---------------------- Middlewares ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.MapControllers();

// ---------------------- Health Check ----------------------
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();
