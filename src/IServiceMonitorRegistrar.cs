namespace ServiceMonitor.Client;

public interface IServiceMonitorRegistrar
{
    /// <summary>
    /// Registers the service with the Service Monitor API
    /// </summary>
    /// <returns>The registered instance ID</returns>
    Task<Guid> RegisterAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the registered instance ID
    /// </summary>
    Guid? InstanceId { get; }
}
