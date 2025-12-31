# ServiceMonitor.Client

[![NuGet](https://img.shields.io/nuget/v/ServiceMonitor.Client.svg)](https://www.nuget.org/packages/ServiceMonitor.Client/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET client library for integrating your services with **ServiceMonitor Dashboard**. Provides automatic service registration, heartbeat monitoring, process metrics collection, and comprehensive deployment tracking with Azure DevOps CI/CD metadata support.

## Features

- **Automatic Service Registration**: Register your service instances automatically on startup
- **Heartbeat Monitoring**: Send periodic heartbeats to track service health
- **Process Metrics**: Collect CPU, memory, and thread count metrics (optional)
- **Port Auto-Detection**: Automatically detect Kestrel/IIS Express listening ports
- **Deployment Tracking**: Track Azure DevOps builds, releases, commits, and branches
- **Retry Logic**: Built-in exponential backoff for failed requests
- **Flexible Configuration**: Configure via code or `appsettings.json`
- **Minimal Dependencies**: Lightweight with minimal external dependencies

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package ServiceMonitor.Client
```

Or via Package Manager Console:

```powershell
Install-Package ServiceMonitor.Client
```

## Quick Start

### 1. Basic Setup (Minimal Configuration)

```csharp
using ServiceMonitor.Client;

var builder = WebApplication.CreateBuilder(args);

// Add ServiceMonitor with minimal configuration
builder.Services.AddServiceMonitor(options =>
{
    options.DashboardUrl = "https://monitor.yourcompany.com";
    options.ApiKey = "sm_live_your_api_key_here";
    options.ServiceName = "my-web-api";
    options.Environment = "production";
});

var app = builder.Build();
app.Run();
```

### 2. Configuration via appsettings.json

**appsettings.json:**
```json
{
  "ServiceMonitor": {
    "DashboardUrl": "https://monitor.yourcompany.com",
    "ApiKey": "sm_live_your_api_key_here",
    "ServiceName": "payment-service",
    "Environment": "production",
    "Version": "1.2.5",
    "HeartbeatInterval": "00:00:30",
    "EnableMetrics": true,
    "EnableLogging": true
  }
}
```

**Program.cs:**
```csharp
builder.Services.AddServiceMonitor(options =>
{
    builder.Configuration.GetSection("ServiceMonitor").Bind(options);
});
```

### 3. Advanced Configuration with Deployment Metadata

Perfect for Azure DevOps or CI/CD environments:

```csharp
builder.Services.AddServiceMonitor(options =>
{
    options.DashboardUrl = "https://monitor.yourcompany.com";
    options.ApiKey = Environment.GetEnvironmentVariable("SERVICE_MONITOR_API_KEY")!;
    options.ServiceName = "payment-service";
    options.Environment = "production";
    options.Version = "1.2.5";

    // Azure DevOps Build Information
    options.BuildId = Environment.GetEnvironmentVariable("BUILD_BUILDID");
    options.ReleaseId = Environment.GetEnvironmentVariable("RELEASE_RELEASEID");
    options.CommitHash = Environment.GetEnvironmentVariable("BUILD_SOURCEVERSION");
    options.Branch = Environment.GetEnvironmentVariable("BUILD_SOURCEBRANCHNAME");
    options.BuildConfiguration = "Release";

    // Deployment Timestamps
    options.BuildDate = DateTime.Parse(Environment.GetEnvironmentVariable("BUILD_DATE") ?? DateTime.UtcNow.ToString());
    options.DeploymentDate = DateTime.UtcNow;

    // Custom Metadata
    options.DeploymentMetadata = new Dictionary<string, object>
    {
        { "deployer", Environment.UserName },
        { "datacenter", "us-west-2" },
        { "cluster", "prod-cluster-01" }
    };

    // Performance Options
    options.HeartbeatInterval = TimeSpan.FromSeconds(30);
    options.EnableMetrics = true;
    options.RetryAttempts = 3;
});
```

## Configuration Options

| Option | Type | Required | Default | Description |
|--------|------|----------|---------|-------------|
| `DashboardUrl` | `string` | Yes | - | Base URL of ServiceMonitor API |
| `ApiKey` | `string` | Yes | - | API key (format: `sm_live_...`) |
| `ServiceName` | `string` | Yes | - | Name of your service |
| `Environment` | `string` | No | `"production"` | Environment (dev, staging, production) |
| `Version` | `string?` | No | `null` | Service version |
| `Hostname` | `string?` | No | Auto-detected | Machine hostname |
| `Port` | `int?` | No | Auto-detected | Service listening port |
| `Url` | `string?` | No | Auto-detected | Service URL |
| `InstanceId` | `string?` | No | Auto-generated | Custom instance identifier |
| `HeartbeatInterval` | `TimeSpan` | No | `30s` | Heartbeat interval (min: 5s) |
| `EnableMetrics` | `bool` | No | `false` | Enable CPU/Memory metrics |
| `RetryAttempts` | `int` | No | `3` | Retry attempts (0-10) |
| `EnableLogging` | `bool` | No | `true` | Enable client logging |

### Deployment Metadata Options

| Option | Type | Description |
|--------|------|-------------|
| `BuildId` | `string?` | Azure DevOps Build ID |
| `ReleaseId` | `string?` | Azure DevOps Release ID |
| `BuildDate` | `DateTime?` | When the artifact was built |
| `DeploymentDate` | `DateTime?` | When this instance was deployed |
| `CommitHash` | `string?` | Git commit hash |
| `Branch` | `string?` | Git branch name |
| `BuildConfiguration` | `string?` | Build configuration (Release/Debug) |
| `DeploymentMetadata` | `Dictionary<string, object>?` | Custom key-value metadata |

## Usage Examples

### Example 1: ASP.NET Core Web API

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddServiceMonitor(options =>
{
    options.DashboardUrl = "https://monitor.example.com";
    options.ApiKey = "sm_live_abc123";
    options.ServiceName = "user-api";
    options.Environment = builder.Environment.EnvironmentName;
    options.Version = "2.1.0";
    options.EnableMetrics = true;
});

var app = builder.Build();
app.MapControllers();
app.Run();
```

### Example 2: Worker Service

```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<MyWorker>();
builder.Services.AddServiceMonitor(options =>
{
    options.DashboardUrl = "https://monitor.example.com";
    options.ApiKey = "sm_live_xyz789";
    options.ServiceName = "background-worker";
    options.Environment = "production";
    options.HeartbeatInterval = TimeSpan.FromMinutes(1);
});

var host = builder.Build();
host.Run();
```

### Example 3: Multiple Instances with Custom InstanceId

```csharp
// Useful when running multiple instances on the same machine
builder.Services.AddServiceMonitor(options =>
{
    options.DashboardUrl = "https://monitor.example.com";
    options.ApiKey = "sm_live_abc123";
    options.ServiceName = "load-balanced-api";
    options.Environment = "production";
    options.InstanceId = $"{Environment.MachineName}-{args[0]}"; // Custom instance ID
    options.Port = int.Parse(args[0]); // Port from command line args
});
```

### Example 4: Azure DevOps Pipeline Integration

**azure-pipelines.yml:**
```yaml
- task: DotNetCoreCLI@2
  displayName: 'Deploy Service'
  inputs:
    command: 'run'
    projects: '**/*.csproj'
  env:
    SERVICE_MONITOR_API_KEY: $(ServiceMonitorApiKey)
    BUILD_BUILDID: $(Build.BuildId)
    RELEASE_RELEASEID: $(Release.ReleaseId)
    BUILD_SOURCEVERSION: $(Build.SourceVersion)
    BUILD_SOURCEBRANCHNAME: $(Build.SourceBranchName)
    BUILD_DATE: $(Build.Date)
```

**Program.cs:**
```csharp
builder.Services.AddServiceMonitor(options =>
{
    options.DashboardUrl = "https://monitor.example.com";
    options.ApiKey = Environment.GetEnvironmentVariable("SERVICE_MONITOR_API_KEY")!;
    options.ServiceName = "my-service";
    options.Environment = "production";

    options.BuildId = Environment.GetEnvironmentVariable("BUILD_BUILDID");
    options.ReleaseId = Environment.GetEnvironmentVariable("RELEASE_RELEASEID");
    options.CommitHash = Environment.GetEnvironmentVariable("BUILD_SOURCEVERSION");
    options.Branch = Environment.GetEnvironmentVariable("BUILD_SOURCEBRANCHNAME");
    options.BuildDate = DateTime.Parse(Environment.GetEnvironmentVariable("BUILD_DATE") ?? DateTime.UtcNow.ToString());
    options.DeploymentDate = DateTime.UtcNow;
});
```

## How It Works

1. **Startup**: When your application starts, `ServiceRegistrationHostedService` automatically registers your service instance with the ServiceMonitor API
2. **Heartbeats**: `HeartbeatBackgroundService` sends periodic heartbeats at the configured interval
3. **Metrics**: If `EnableMetrics` is enabled, each heartbeat includes CPU, memory, and thread count data
4. **Shutdown**: When your application stops, heartbeats cease and the dashboard marks the instance as unhealthy

## Port Auto-Detection

The client automatically detects the listening port from:
- Kestrel configuration
- IIS Express settings
- ASP.NET Core server addresses

If auto-detection fails, it falls back to using Process ID for instance identification.

## Logging

The client uses `ILogger<T>` for logging. Logs include:
- Service registration events
- Heartbeat successes/failures
- Retry attempts
- Error messages

Disable logging by setting `EnableLogging = false`.

## Error Handling

- **Registration Failures**: If registration fails, heartbeats won't start
- **Heartbeat Failures**: Failed heartbeats trigger exponential backoff retry logic
- **Network Issues**: Retries up to `RetryAttempts` times with exponential delays (2s, 4s, 8s, ...)

## Best Practices

1. **Store API Keys Securely**: Use environment variables or Azure Key Vault, never hardcode
2. **Set Appropriate Heartbeat Intervals**: Balance between freshness and API load (recommended: 30-60s)
3. **Enable Metrics in Production**: Helps diagnose performance issues
4. **Use Deployment Metadata**: Correlate deployments with incidents
5. **Configure Logging**: Adjust log levels based on environment

## Troubleshooting

### Service Not Appearing in Dashboard

- Verify `DashboardUrl` is correct and accessible
- Check API key format: `sm_live_...`
- Review logs for registration errors
- Ensure firewall allows outbound HTTPS

### Heartbeats Not Updating

- Check that registration succeeded (look for "Successfully registered" log)
- Verify network connectivity to dashboard
- Ensure service hasn't crashed after startup

### Port Detection Issues

- Manually set `Port` option if auto-detection fails
- Check Kestrel/IIS configuration
- Verify `IServer` is properly registered in DI

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- **Issues**: [GitHub Issues](https://github.com/ikeskin/ServiceMonitor.Client/issues)
- **Documentation**: [GitHub Wiki](https://github.com/ikeskin/ServiceMonitor.Client/wiki)

## Contributing

Contributions are welcome! Please open an issue or pull request.

---
