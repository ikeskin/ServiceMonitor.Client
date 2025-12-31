# Web API Sample

This sample demonstrates how to integrate ServiceMonitor.Client with an ASP.NET Core Web API.

## Prerequisites

- .NET 8.0 SDK
- ServiceMonitor Dashboard running (default: http://localhost:5192)
- Valid API key from ServiceMonitor Dashboard

## Configuration

Update `appsettings.json` with your ServiceMonitor Dashboard URL and API key:

```json
{
  "ServiceMonitor": {
    "DashboardUrl": "http://localhost:5192",
    "ApiKey": "sm_live_your_api_key_here",
    "ServiceName": "web-api-sample",
    "Environment": "development"
  }
}
```

## Running the Sample

```bash
dotnet run
```

The API will start on `https://localhost:5001` (or the configured port).

## Endpoints

- `GET /` - Root endpoint with service information
- `GET /health` - Health check endpoint
- `GET /swagger` - Swagger UI (development only)

## What Happens

1. On startup, the service registers itself with ServiceMonitor Dashboard
2. Every 30 seconds, a heartbeat is sent with process metrics (CPU, Memory)
3. You can view the service status in the ServiceMonitor Dashboard
4. When you stop the service, heartbeats cease and the dashboard marks it as unhealthy

## Deployment Metadata

The sample also demonstrates how to capture Azure DevOps build metadata:

```csharp
options.BuildId = Environment.GetEnvironmentVariable("BUILD_BUILDID");
options.CommitHash = Environment.GetEnvironmentVariable("BUILD_SOURCEVERSION");
options.Branch = Environment.GetEnvironmentVariable("BUILD_SOURCEBRANCHNAME");
```

These are automatically populated in Azure Pipelines.
