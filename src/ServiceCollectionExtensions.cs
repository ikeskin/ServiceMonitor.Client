using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServiceMonitor.Client;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Service Monitor client to the service collection.
    /// This will register the service with the Service Monitor API and send periodic heartbeats.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Configuration action for ServiceMonitorOptions</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddServiceMonitor(
        this IServiceCollection services,
        Action<ServiceMonitorOptions> configure)
    {
        // Validate configuration
        var options = new ServiceMonitorOptions();
        configure(options);

        ValidateOptions(options);

        // Register options
        services.Configure(configure);

        // Register HttpClient for registrar
        services.AddHttpClient<IServiceMonitorRegistrar, ServiceMonitorRegistrar>();

        // Register HttpClient for heartbeat service
        services.AddHttpClient<HeartbeatBackgroundService>();

        // Register registrar as singleton so instance ID is shared
        services.AddSingleton<IServiceMonitorRegistrar, ServiceMonitorRegistrar>();

        // Register background service for registration
        services.AddHostedService<ServiceRegistrationHostedService>();

        // Register background service for heartbeats
        services.AddHostedService<HeartbeatBackgroundService>();

        return services;
    }

    private static void ValidateOptions(ServiceMonitorOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.DashboardUrl))
        {
            throw new ArgumentException("DashboardUrl is required", nameof(options.DashboardUrl));
        }

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new ArgumentException("ApiKey is required", nameof(options.ApiKey));
        }

        if (string.IsNullOrWhiteSpace(options.ServiceName))
        {
            throw new ArgumentException("ServiceName is required", nameof(options.ServiceName));
        }

        if (!Uri.TryCreate(options.DashboardUrl, UriKind.Absolute, out _))
        {
            throw new ArgumentException("DashboardUrl must be a valid absolute URL", nameof(options.DashboardUrl));
        }

        if (options.HeartbeatInterval < TimeSpan.FromSeconds(5))
        {
            throw new ArgumentException("HeartbeatInterval must be at least 5 seconds", nameof(options.HeartbeatInterval));
        }

        if (options.RetryAttempts < 0 || options.RetryAttempts > 10)
        {
            throw new ArgumentException("RetryAttempts must be between 0 and 10", nameof(options.RetryAttempts));
        }
    }
}
