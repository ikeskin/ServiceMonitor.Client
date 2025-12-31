using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Logging;

namespace ServiceMonitor.Client;

internal static class PortDetector
{
    /// <summary>
    /// Attempts to detect the listening port from IServerAddressesFeature
    /// Waits up to 2 seconds for the addresses to be populated
    /// </summary>
    public static async Task<int?> DetectPortAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to get IServer from DI
            var server = serviceProvider.GetService(typeof(IServer)) as IServer;
            if (server == null)
                return null;

            // Get the addresses feature
            var addressesFeature = server.Features.Get<IServerAddressesFeature>();
            if (addressesFeature == null)
                return null;

            // Wait for addresses to be populated (up to 2 seconds)
            var maxAttempts = 20;
            var delay = 100; // ms
            for (int i = 0; i < maxAttempts; i++)
            {
                if (addressesFeature.Addresses.Any())
                    break;

                await Task.Delay(delay, cancellationToken);
            }

            if (!addressesFeature.Addresses.Any())
                return null;

            // Parse the first address to get the port
            var address = addressesFeature.Addresses.FirstOrDefault();
            if (string.IsNullOrEmpty(address))
                return null;

            // Address format: http://localhost:5000 or https://localhost:5001
            var uri = new Uri(address);
            return uri.Port;
        }
        catch
        {
            // If anything fails, return null and fall back to process ID
            return null;
        }
    }

    /// <summary>
    /// Attempts to detect the full URL from IServerAddressesFeature
    /// Waits up to 2 seconds for the addresses to be populated
    /// </summary>
    public static async Task<string?> DetectUrlAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        try
        {
            var server = serviceProvider.GetService(typeof(IServer)) as IServer;
            if (server == null)
                return null;

            var addressesFeature = server.Features.Get<IServerAddressesFeature>();
            if (addressesFeature == null)
                return null;

            // Wait for addresses to be populated (up to 2 seconds)
            var maxAttempts = 20;
            var delay = 100; // ms
            for (int i = 0; i < maxAttempts; i++)
            {
                if (addressesFeature.Addresses.Any())
                    break;

                await Task.Delay(delay, cancellationToken);
            }

            // Return the first address
            return addressesFeature.Addresses.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }
}
