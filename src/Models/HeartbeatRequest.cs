namespace ServiceMonitor.Client.Models;

internal class HeartbeatRequest
{
    public Guid InstanceId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
