# Tech Stack

## Cloud Infrastructure

**Not Applicable** - This is an offline desktop application with zero cloud dependencies. No cloud provider, no cloud services, no deployment regions.

## Technology Stack Table

| Category | Technology | Version | Purpose | Rationale |
|----------|-----------|---------|---------|-----------|
| **Language** | C# | 12.0 | Primary development language | Latest C# version included with .NET 8.0, provides modern language features (primary constructors, collection expressions), required for WinUI3 development |
| **Runtime** | .NET | 8.0 LTS | Application runtime | Long-term support until Nov 2026, stable performance, Windows desktop optimization, includes Windows SDK integration |
| **Framework** | Windows App SDK (WinUI3) | 1.5.240802000 | UI framework and Windows API access | Latest stable release, provides WinUI3 controls, Fluent Design System, Windows.Devices.Enumeration API access, Standard-User compatible APIs |
| **MVVM Toolkit** | CommunityToolkit.Mvvm | 8.2.2 | MVVM pattern implementation | Official Microsoft toolkit, source generators reduce boilerplate, ObservableObject/RelayCommand base classes, actively maintained |
| **Unit Test Framework** | xUnit | 2.6.6 | Test execution engine | Industry standard for .NET, excellent VS Code integration, parallel test execution, strongly-typed assertions |
| **Assertion Library** | FluentAssertions | 6.12.0 | Readable test assertions | Improves test readability with natural language syntax, comprehensive assertion library, reduces test code verbosity |
| **Mocking Framework** | NSubstitute | 5.1.0 | Test doubles and mocking | Simplest mocking syntax in .NET ecosystem, excellent for mocking service dependencies, integrates with xUnit |
| **Test Runner** | xUnit Visual Studio Test Platform | 2.5.6 | VS Code test integration | Enables Test Explorer UI in VS Code, provides debugging integration, required for C# Dev Kit test discovery |
| **Dependency Injection** | Microsoft.Extensions.DependencyInjection | 8.0.0 | IoC container | Standard .NET DI container, lightweight, constructor injection support, integrates with WinUI3 App.xaml.cs |
| **Development Tools** | VS Code + C# Dev Kit | 1.0+ | Primary IDE | Lightweight development environment, excellent Git integration, XAML syntax highlighting, .NET CLI integration |
| **SDK** | .NET SDK | 8.0.101 | Build tools and compiler | Includes C# 12 compiler, `dotnet` CLI tools, Windows 10 SDK (10.0.19041.0), project templates |
| **Package Manager** | NuGet | 6.8+ | Dependency management | Integrated with .NET CLI and VS Code, standard for .NET ecosystem |
| **Version Control** | Git | 2.43+ | Source control | Industry standard VCS, GitHub integration |
| **CI/CD** | GitHub Actions | N/A | Automated builds and tests | Free for public repos, same CLI commands as local development, artifact publishing |
| **Code Analysis** | .NET Compiler Platform (Roslyn) Analyzers | 8.0.0 | Static code analysis | Built into .NET SDK, enforces C# best practices, nullability analysis, async/await pattern validation |
| **Logging (Phase 2)** | _Deferred_ | N/A | Diagnostic logging | MVP uses `Debug.WriteLine` only, Serilog with file sink planned for post-MVP troubleshooting |
