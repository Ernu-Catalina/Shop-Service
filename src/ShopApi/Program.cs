using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Repos;
using ShopApi.Services;
using Roleplay.Grpc;
using Grpc.Net.ClientFactory;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<ShopDbContext>(o => o.UseNpgsql(connectionString));

// Redis
var redisConfig = builder.Configuration["Redis:Configuration"] ?? builder.Configuration["REDIS__CONFIG"] ?? "localhost:6379";
builder.Services.AddStackExchangeRedisCache(o => o.Configuration = redisConfig);

// gRPC client for Roleplay service
var roleplayAddress = builder.Configuration["ROLEPLAY_GRPC_URL"] ?? "http://roleplay:50051";
builder.Services.AddGrpcClient<RoleplayService.RoleplayServiceClient>(o => o.Address = new Uri(roleplayAddress));

// Register application services
builder.Services.AddScoped<ICharacterClient, CharacterClient>(); // gRPC-backed wrapper
builder.Services.AddScoped<ItemRepository>();
builder.Services.AddHostedService<ServiceRegistration>();

// General HTTP client factory (keep for other uses)
builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure DB with retry loop
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    var retries = 10;
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
app.MapGet("/health", () => Results.Json(new { status = "healthy" }));

app.Run();