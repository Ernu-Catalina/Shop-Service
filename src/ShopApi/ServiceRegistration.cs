using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ServiceRegistration : IHostedService, IDisposable
{
    private readonly IConfiguration _config;
    private readonly ILogger<ServiceRegistration> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private Timer? _heartbeatTimer;
    private string _serviceId = Guid.NewGuid().ToString();

    public ServiceRegistration(IConfiguration config, ILogger<ServiceRegistration> logger, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var serviceName = _config["SERVICE_NAME"] ?? "unknown-service";
        var port = _config["PORT"] ?? "80";
        var discoveryUrl = _config["SERVICE_DISCOVERY_URL"] ?? "http://service_discovery:8500";

        var registrationData = new
        {
            service_name = serviceName,
            service_id = _serviceId,
            host = Environment.MachineName,
            port = int.Parse(port),
            health_check_url = "/health",
            metadata = new
            {
                version = "1.0.0",
                environment = _config["ASPNETCORE_ENVIRONMENT"] ?? "development"
            }
        };

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{discoveryUrl}/register", registrationData, cancellationToken);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Service registered successfully with Service Discovery.");

            // Start heartbeats every 30s
            _heartbeatTimer = new Timer(async _ => await SendHeartbeat(discoveryUrl), null, 30000, 30000);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register service.");
        }
    }

    private async Task SendHeartbeat(string discoveryUrl)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            await client.PostAsJsonAsync($"{discoveryUrl}/heartbeat", new { service_id = _serviceId });
            _logger.LogInformation("Heartbeat sent.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Heartbeat failed.");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _heartbeatTimer?.Dispose();

        var discoveryUrl = _config["SERVICE_DISCOVERY_URL"] ?? "http://service_discovery:8500";
        try
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync($"{discoveryUrl}/deregister/{_serviceId}", cancellationToken);
            _logger.LogInformation("Service deregistered successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deregister service.");
        }
    }

    public void Dispose()
    {
        _heartbeatTimer?.Dispose();
    }
}
