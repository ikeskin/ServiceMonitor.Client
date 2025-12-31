# Contributing to ServiceMonitor.Client

Thank you for your interest in contributing to ServiceMonitor.Client! We welcome contributions from the community.

## Getting Started

1. Fork the repository
2. Clone your fork: `git clone https://github.com/YOUR_USERNAME/ServiceMonitor.Client.git`
3. Create a feature branch: `git checkout -b feature/your-feature-name`
4. Make your changes
5. Run tests (if any): `dotnet test`
6. Commit your changes: `git commit -m "Add your feature"`
7. Push to your fork: `git push origin feature/your-feature-name`
8. Open a Pull Request

## Development Setup

### Prerequisites
- .NET 8.0 SDK or later
- Your favorite IDE (Visual Studio, VS Code, Rider)

### Building the Project
```bash
dotnet restore
dotnet build
```

### Running the Sample
```bash
cd samples/WebApiSample
dotnet run
```

## Coding Guidelines

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods small and focused
- Write unit tests for new features

## Pull Request Guidelines

- Ensure your PR has a clear description of the changes
- Reference any related issues
- Keep PRs focused on a single feature or fix
- Update CHANGELOG.md with your changes
- Ensure all tests pass
- Update documentation if needed

## Reporting Issues

- Use GitHub Issues to report bugs or request features
- Provide a clear description and steps to reproduce
- Include relevant logs or error messages
- Specify your .NET version and operating system

## Code of Conduct

- Be respectful and inclusive
- Welcome newcomers and help them get started
- Focus on constructive feedback
- Assume good intentions

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

Thank you for contributing!
