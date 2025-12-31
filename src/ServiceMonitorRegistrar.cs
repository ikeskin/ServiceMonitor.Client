using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceMonitor.Client.Models;

namespace ServiceMonitor.Client;

internal class ServiceMonitorRegistrar : IServiceMonitorRegistrar
{
    private readonly HttpClient _httpClient;
    private readonly ServiceMonitorOptions _options;
    private readonly ILogger<ServiceMonitorRegistrar> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Guid? _instanceId;

    public Guid? InstanceId => _instanceId;

    public ServiceMonitorRegistrar(
        HttpClient httpClient,
        IOptions<ServiceMonitorOptions> options,
        ILogger<ServiceMonitorRegistrar> logger,
        IServiceProvider serviceProvider)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        _serviceProvider = serviceProvider;

        // Configure HttpClient
        _httpClient.BaseAddress = new Uri(_options.DashboardUrl);
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _options.ApiKey);
    }

    public async Task<Guid> RegisterAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Auto-detect hostname if not provided
            var hostname = _options.Hostname ?? System.Environment.MachineName;

            // Get process ID
            var processId = System.Diagnostics.Process.GetCurrentProcess().Id;

            // Auto-detect listening port from Kestrel/IIS Express
            int? port = _options.Port ?? await PortDetector.DetectPortAsync(_serviceProvider, cancellationToken);

            // Auto-detect URL if not provided
            string? url = _options.Url ?? await PortDetector.DetectUrlAsync(_serviceProvider, cancellationToken);

            // Build comprehensive metadata including deployment info
            var metadata = new Dictionary<string, object>
            {
                { "sdk_version", "1.0.0" },
                { "framework", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription },
                { "os", System.Runtime.InteropServices.RuntimeInformation.OSDescription },
                { "process_id", processId },
                { "registered_at", DateTime.UtcNow }
            };

            // Add deployment metadata if provided
            if (!string.IsNullOrEmpty(_options.BuildId))
                metadata["build_id"] = _options.BuildId;

            if (!string.IsNullOrEmpty(_options.ReleaseId))
                metadata["release_id"] = _options.ReleaseId;

            if (_options.BuildDate.HasValue)
                metadata["build_date"] = _options.BuildDate.Value;

            if (_options.DeploymentDate.HasValue)
                metadata["deployment_date"] = _options.DeploymentDate.Value;

            if (!string.IsNullOrEmpty(_options.CommitHash))
                metadata["commit_hash"] = _options.CommitHash;

            if (!string.IsNullOrEmpty(_options.Branch))
                metadata["branch"] = _options.Branch;

            if (!string.IsNullOrEmpty(_options.BuildConfiguration))
                metadata["build_configuration"] = _options.BuildConfiguration;

            // Merge custom deployment metadata
            if (_options.DeploymentMetadata != null)
            {
                foreach (var kvp in _options.DeploymentMetadata)
                {
                    metadata[$"custom_{kvp.Key}"] = kvp.Value;
                }
            }

            var request = new ServiceRegistrationRequest
            {
                ServiceName = _options.ServiceName,
                Environment = _options.Environment,
                Version = _options.Version,
                InstanceId = _options.InstanceId, // Can be null - server will generate based on hostname+port
                Hostname = hostname,
                Port = port,
                Url = url,
                ProcessId = processId,
                Metadata = metadata
            };

            if (_options.EnableLogging)
            {
                var logicalId = _options.InstanceId ?? (port.HasValue ? $"{hostname}:{port}" : $"{hostname}-pid{processId}");
                _logger.LogInformation(
                    "Registering service {ServiceName} ({Environment}) as instance {InstanceId} (Port: {Port}, ProcessId: {ProcessId})",
                    _options.ServiceName,
                    _options.Environment,
                    logicalId,
                    port?.ToString() ?? "auto-detect",
                    processId);
            }

            var response = await _httpClient.PostAsJsonAsync("/api/services/register", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ServiceRegistrationResponse>(cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to parse registration response");
            }

            _instanceId = result.InstanceId;

            if (_options.EnableLogging)
            {
                _logger.LogInformation(
                    "Successfully registered service {ServiceName}. Instance ID: {InstanceId}",
                    _options.ServiceName,
                    _instanceId);
            }

            return _instanceId.Value;
        }
        catch (HttpRequestException ex)
        {
            if (_options.EnableLogging)
            {
                _logger.LogError(ex, "Failed to register service with Service Monitor API");
            }
            throw new InvalidOperationException("Failed to register service with Service Monitor API. Please check the DashboardUrl and ApiKey configuration.", ex);
        }
        catch (Exception ex)
        {
            if (_options.EnableLogging)
            {
                _logger.LogError(ex, "Unexpected error during service registration");
            }
            throw;
        }
    }
}
