using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceMonitor.Client.Models;

namespace ServiceMonitor.Client;

internal class HeartbeatBackgroundService : BackgroundService
{
    private readonly HttpClient _httpClient;
    private readonly IServiceMonitorRegistrar _registrar;
    private readonly ServiceMonitorOptions _options;
    private readonly ILogger<HeartbeatBackgroundService> _logger;
    private readonly Process _currentProcess;

    public HeartbeatBackgroundService(
        HttpClient httpClient,
        IServiceMonitorRegistrar registrar,
        IOptions<ServiceMonitorOptions> options,
        ILogger<HeartbeatBackgroundService> logger)
    {
        _httpClient = httpClient;
        _registrar = registrar;
        _options = options.Value;
        _logger = logger;
        _currentProcess = Process.GetCurrentProcess();

        // Configure HttpClient
        _httpClient.BaseAddress = new Uri(_options.DashboardUrl);
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _options.ApiKey);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for service registration to complete
        await WaitForRegistrationAsync(stoppingToken);

        if (_registrar.InstanceId == null)
        {
            if (_options.EnableLogging)
            {
                _logger.LogError("Service registration failed. Heartbeat service cannot start.");
            }
            return;
        }

        if (_options.EnableLogging)
        {
            _logger.LogInformation(
                "Heartbeat service started for instance {InstanceId}. Interval: {Interval}",
                _registrar.InstanceId,
                _options.HeartbeatInterval);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendHeartbeatAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                if (_options.EnableLogging)
                {
                    _logger.LogError(ex, "Error sending heartbeat");
                }
            }

            await Task.Delay(_options.HeartbeatInterval, stoppingToken);
        }

        if (_options.EnableLogging)
        {
            _logger.LogInformation("Heartbeat service stopped");
        }
    }

    private async Task WaitForRegistrationAsync(CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromSeconds(30);
        var elapsed = TimeSpan.Zero;
        var checkInterval = TimeSpan.FromMilliseconds(100);

        while (_registrar.InstanceId == null && elapsed < timeout)
        {
            await Task.Delay(checkInterval, cancellationToken);
            elapsed += checkInterval;
        }
    }

    private async Task SendHeartbeatAsync(CancellationToken cancellationToken)
    {
        var metadata = new Dictionary<string, object>
        {
            { "timestamp", DateTime.UtcNow }
        };

        // Collect metrics if enabled
        if (_options.EnableMetrics)
        {
            try
            {
                _currentProcess.Refresh();

                // CPU percentage (approximation)
                var cpuTime = _currentProcess.TotalProcessorTime;
                var uptime = DateTime.UtcNow - _currentProcess.StartTime.ToUniversalTime();
                var cpuPercent = (cpuTime.TotalMilliseconds / uptime.TotalMilliseconds) * 100.0 / Environment.ProcessorCount;

                // Memory in MB
                var memoryMB = _currentProcess.WorkingSet64 / 1024.0 / 1024.0;

                metadata["cpu_percent"] = Math.Round(cpuPercent, 2);
                metadata["memory_mb"] = Math.Round(memoryMB, 2);
                metadata["thread_count"] = _currentProcess.Threads.Count;
            }
            catch (Exception ex)
            {
                if (_options.EnableLogging)
                {
                    _logger.LogWarning(ex, "Failed to collect process metrics");
                }
            }
        }

        var request = new HeartbeatRequest
        {
            InstanceId = _registrar.InstanceId!.Value,
            Metadata = metadata
        };

        var retryCount = 0;
        var maxRetries = _options.RetryAttempts;

        while (retryCount <= maxRetries)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/services/heartbeat", request, cancellationToken);
                response.EnsureSuccessStatusCode();

                if (_options.EnableLogging)
                {
                    _logger.LogDebug("Heartbeat sent successfully for instance {InstanceId}", _registrar.InstanceId);
                }

                return;
            }
            catch (HttpRequestException ex)
            {
                retryCount++;

                if (retryCount > maxRetries)
                {
                    if (_options.EnableLogging)
                    {
                        _logger.LogError(ex, "Failed to send heartbeat after {RetryCount} attempts", maxRetries);
                    }
                    return;
                }

                // Exponential backoff
                var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
                if (_options.EnableLogging)
                {
                    _logger.LogWarning(
                        "Heartbeat failed (attempt {Attempt}/{MaxAttempts}). Retrying in {Delay}s...",
                        retryCount,
                        maxRetries,
                        delay.TotalSeconds);
                }

                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}
