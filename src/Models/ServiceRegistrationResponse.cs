namespace ServiceMonitor.Client.Models;

internal class ServiceRegistrationResponse
{
    public Guid ServiceId { get; set; }
    public Guid InstanceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
