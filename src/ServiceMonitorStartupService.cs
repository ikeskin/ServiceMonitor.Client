using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServiceMonitor.Client;

/// <summary>
/// Background service that updates port information after the application has fully started
/// This ensures IServerAddressesFeature is populated before we try to detect the port
/// </summary>
internal class ServiceMonitorStartupService : IHostedService
{
    private readonly IServiceMonitorRegistrar _registrar;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<ServiceMonitorStartupService> _logger;

    public ServiceMonitorStartupService(
        IServiceMonitorRegistrar registrar,
        IServiceProvider serviceProvider,
        IHostApplicationLifetime lifetime,
        ILogger<ServiceMonitorStartupService> logger)
    {
        _registrar = registrar;
        _serviceProvider = serviceProvider;
        _lifetime = lifetime;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Register a callback for when the application has fully started
        _lifetime.ApplicationStarted.Register(async () =>
        {
            try
            {
                // Now that the app is running, we can detect the port
                var port = await PortDetector.DetectPortAsync(_serviceProvider, cancellationToken);
                var url = await PortDetector.DetectUrlAsync(_serviceProvider, cancellationToken);

                if (port.HasValue || !string.IsNullOrEmpty(url))
                {
                    _logger.LogInformation("Detected port: {Port}, URL: {Url}", port, url);
                    // TODO: Implement an update endpoint to send port/URL to the API
                    // For now, the initial registration will use ProcessId-based instance ID
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to detect port after application startup");
            }
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
