# ServiceMonitor.Client

A .NET client library for integrating applications with the Service Monitor platform. Automatically registers your service and sends periodic heartbeats to track uptime and health.

## Features

- 🚀 **Easy Integration** - Add monitoring with just 3 lines of code
- 📊 **Automatic Metrics** - Optionally collects CPU, Memory, and Thread count
- 🔄 **Auto-Retry** - Built-in retry logic with exponential backoff
- ⚙️ **Configurable** - Customize heartbeat intervals, logging, and more
- 🎯 **Lightweight** - Minimal overhead on your application

## Installation

```bash
dotnet add package ServiceMonitor.Client
```

## Quick Start

Add Service Monitor to your ASP.NET Core application:

```csharp
using ServiceMonitor.Client;

var builder = WebApplication.CreateBuilder(args);

// Add Service Monitor
builder.Services.AddServiceMonitor(options =>
{
    options.DashboardUrl = "https://api.servicemonitor.io";
    options.ApiKey = "sm_live_your_api_key_here";
    options.ServiceName = "my-api";
    options.Environment = "production";
});

var app = builder.Build();
app.Run();
```

That's it! Your service will automatically:
1. Register with the Service Monitor on startup
2. Send heartbeats every 30 seconds
3. Report health status and metrics

## Configuration Options

```csharp
builder.Services.AddServiceMonitor(options =>
{
    // Required
    options.DashboardUrl = "https://api.servicemonitor.io";  // Service Monitor API URL
    options.ApiKey = "sm_live_...";                          // Your API key
    options.ServiceName = "my-service";                      // Name of your service

    // Optional
    options.Environment = "production";                      // Environment (default: "production")
    options.Version = "1.0.0";                              // Service version
    options.Hostname = "web-server-1";                      // Auto-detected if not provided
    options.InstanceId = "unique-id";                       // Auto-generated if not provided
    options.HeartbeatInterval = TimeSpan.FromSeconds(30);   // Heartbeat frequency (default: 30s)
    options.EnableMetrics = true;                           // Collect CPU/Memory metrics (default: false)
    options.EnableLogging = true;                           // Enable logging (default: true)
    options.RetryAttempts = 3;                              // Retry attempts for failed requests (default: 3)
});
```

## Getting Your API Key

1. Sign up at [Service Monitor](https://servicemonitor.io)
2. Navigate to **API Keys** in your dashboard
3. Click **Create New Key**
4. Copy the key (starts with `sm_live_`)
5. **Important:** The key is shown only once - save it securely!

## Usage Examples

### ASP.NET Core Web API

```csharp
using ServiceMonitor.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceMonitor(options =>
{
    options.DashboardUrl = builder.Configuration["ServiceMonitor:Url"];
    options.ApiKey = builder.Configuration["ServiceMonitor:ApiKey"];
    options.ServiceName = "payment-api";
    options.Environment = builder.Environment.EnvironmentName.ToLower();
    options.EnableMetrics = true;
});

var app = builder.Build();
app.Run();
```

### Worker Service

```csharp
using ServiceMonitor.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddServiceMonitor(options =>
{
    options.DashboardUrl = "https://api.servicemonitor.io";
    options.ApiKey = "sm_live_...";
    options.ServiceName = "background-worker";
    options.Environment = "production";
    options.HeartbeatInterval = TimeSpan.FromMinutes(1);
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
```

### Using appsettings.json

**appsettings.json:**
```json
{
  "ServiceMonitor": {
    "DashboardUrl": "https://api.servicemonitor.io",
    "ApiKey": "sm_live_your_api_key",
    "ServiceName": "my-service",
    "Environment": "production",
    "EnableMetrics": true
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

## Metrics Collection

When `EnableMetrics = true`, the client collects:

- **CPU Usage** - Percentage of CPU used by the process
- **Memory Usage** - Working set memory in MB
- **Thread Count** - Number of active threads

These metrics are sent with each heartbeat and visible in the Service Monitor dashboard.

## Error Handling

The client is designed to fail gracefully:
- If registration fails, the application continues running but won't be monitored
- If heartbeat fails, it retries with exponential backoff (up to 3 attempts by default)
- All errors are logged (can be disabled with `EnableLogging = false`)

## How It Works

1. **On Startup:** The client registers your service with the Service Monitor API
2. **Background Service:** A background service sends heartbeats at the configured interval
3. **Heartbeat:** Each heartbeat updates the last-seen timestamp and optionally includes metrics
4. **Monitoring:** If heartbeats stop for 2+ minutes, the service is marked as "down"

## Requirements

- .NET 8.0 or higher
- Active Service Monitor account
- Valid API key

## Support

- Documentation: [https://docs.servicemonitor.io](https://docs.servicemonitor.io)
- Issues: [GitHub Issues](https://github.com/servicemonitor/client-dotnet/issues)
- Email: support@servicemonitor.io

## License

MIT License - see LICENSE file for details
