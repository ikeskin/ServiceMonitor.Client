namespace ServiceMonitor.Client;

public class ServiceMonitorOptions
{
    /// <summary>
    /// The base URL of the Service Monitor API (e.g., http://localhost:5192 or https://api.servicemonitor.io)
    /// </summary>
    public string DashboardUrl { get; set; } = string.Empty;

    /// <summary>
    /// API key for authenticating with the Service Monitor API (format: sm_live_...)
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Name of the service being monitored (e.g., "web-api", "payment-service")
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Environment where the service is running (e.g., "dev", "staging", "production")
    /// </summary>
    public string Environment { get; set; } = "production";

    /// <summary>
    /// Version of the service (optional)
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Hostname of the machine running the service (auto-detected if not provided)
    /// </summary>
    public string? Hostname { get; set; }

    // Deployment Metadata (Azure DevOps, CI/CD)

    /// <summary>
    /// Azure DevOps Build ID (e.g., "12345")
    /// </summary>
    public string? BuildId { get; set; }

    /// <summary>
    /// Azure DevOps Release ID (e.g., "67890")
    /// </summary>
    public string? ReleaseId { get; set; }

    /// <summary>
    /// Build/Release date and time (when the artifact was created)
    /// </summary>
    public DateTime? BuildDate { get; set; }

    /// <summary>
    /// Deployment date and time (when this instance was deployed)
    /// </summary>
    public DateTime? DeploymentDate { get; set; }

    /// <summary>
    /// Git commit hash (e.g., "a1b2c3d4e5f6...")
    /// </summary>
    public string? CommitHash { get; set; }

    /// <summary>
    /// Git branch name (e.g., "main", "develop", "release/1.0")
    /// </summary>
    public string? Branch { get; set; }

    /// <summary>
    /// Build configuration (e.g., "Release", "Debug")
    /// </summary>
    public string? BuildConfiguration { get; set; }

    /// <summary>
    /// Custom deployment metadata (key-value pairs for any additional information)
    /// </summary>
    public Dictionary<string, object>? DeploymentMetadata { get; set; }

    /// <summary>
    /// Port number the service is listening on (optional, used for instance identification)
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// Full URL of the service (optional, e.g., http://localhost:5000)
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Custom instance identifier (optional, auto-generated from Hostname+Port if not provided)
    /// Format: hostname:port or hostname-pidXXXX
    /// </summary>
    public string? InstanceId { get; set; }

    /// <summary>
    /// Interval between heartbeat requests
    /// </summary>
    public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Enable collection of process metrics (CPU, Memory)
    /// </summary>
    public bool EnableMetrics { get; set; } = false;

    /// <summary>
    /// Number of retry attempts for failed API calls
    /// </summary>
    public int RetryAttempts { get; set; } = 3;

    /// <summary>
    /// Enable logging for the Service Monitor client
    /// </summary>
    public bool EnableLogging { get; set; } = true;
}
