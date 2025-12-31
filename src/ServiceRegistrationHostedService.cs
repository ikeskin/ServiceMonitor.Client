using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServiceMonitor.Client;

internal class ServiceRegistrationHostedService : IHostedService
{
    private readonly IServiceMonitorRegistrar _registrar;
    private readonly ILogger<ServiceRegistrationHostedService> _logger;

    public ServiceRegistrationHostedService(
        IServiceMonitorRegistrar registrar,
        ILogger<ServiceRegistrationHostedService> logger)
    {
        _registrar = registrar;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _registrar.RegisterAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register service with Service Monitor. The application will continue but monitoring will not work.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
