# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-12-31

### Added
- Initial release of ServiceMonitor.Client
- Automatic service registration with ServiceMonitor Dashboard
- Heartbeat monitoring with configurable intervals
- Process metrics collection (CPU, Memory, Thread count)
- Automatic port detection for Kestrel/IIS Express
- Azure DevOps deployment metadata support (Build ID, Release ID, Commit Hash, Branch)
- Retry logic with exponential backoff
- Comprehensive configuration options via code or appsettings.json
- XML documentation for IntelliSense support
- MIT License

### Features
- **Service Registration**: Automatically register service instances on startup
- **Heartbeat Monitoring**: Send periodic heartbeats to track service health
- **Deployment Tracking**: Track CI/CD deployments with metadata
- **Flexible Configuration**: Configure via fluent API or configuration files
- **Auto-Detection**: Automatically detect hostname, port, and URLs
- **Error Handling**: Built-in retry logic and comprehensive logging

[1.0.0]: https://github.com/ikeskin/ServiceMonitor.Client/releases/tag/v1.0.0
