namespace ServiceMonitor.Client.Models;

internal class ServiceRegistrationRequest
{
    public string ServiceName { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string? InstanceId { get; set; } // Optional - will be auto-generated if not provided
    public string? Hostname { get; set; }
    public int? Port { get; set; }
    public string? Url { get; set; }
    public int? ProcessId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
