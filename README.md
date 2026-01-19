[![Build Status](https://github.com/dernerl/UsbDeviceInspector/actions/workflows/build.yml/badge.svg)](https://github.com/dernerl/UsbDeviceInspector/actions/workflows/build.yml)

# USB Device Inspector

A desktop application for IT administrators to enumerate USB storage devices and extract hardware identifiers. This offline tool generates formatted output for CrowdStrike Falcon Device Control policies and helpdesk ticketing systems, eliminating manual device identification workflows.

Built with .NET 8.0 and WinUI3, USB Device Inspector provides a modern Fluent Design interface for Windows 10/11 environments with zero telemetry or cloud dependencies.

## Prerequisites

Before setting up the project, ensure you have the following installed:

| Requirement | Version | Notes |
|-------------|---------|-------|
| **Windows** | 10 21H2+ or 11 | Windows 10 SDK 10.0.19041.0 included with .NET SDK |
| **.NET SDK** | 8.0.101+ | Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0) |
| **VS Code** | Latest | Primary IDE - download from [code.visualstudio.com](https://code.visualstudio.com/) |
| **Git** | 2.43+ | Download from [git-scm.com](https://git-scm.com/) |

**Alternative IDE:** Visual Studio 2022 (17.8+) with .NET desktop development workload.

## VS Code Extensions

### Required Extensions

Install these extensions for full development support:

- **[C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)** - Full C# language support, IntelliSense, debugging, and test explorer
- **[C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)** - Base C# language support (installed automatically with C# Dev Kit)

### Recommended Extensions

- **[GitLens](https://marketplace.visualstudio.com/items?itemName=eamodio.gitlens)** - Enhanced Git integration and history visualization

> **Note:** Opening this project in VS Code will prompt you to install recommended extensions (configured in `.vscode/extensions.json`).

## Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/dernerl/UsbDeviceInspector.git
cd UsbDeviceInspector
```

### 2. Install Windows App SDK Templates

```bash
dotnet new install Microsoft.WindowsAppSDK.Templates
```

### 3. Restore Dependencies

```bash
dotnet restore UsbDeviceInspector.sln
```

### 4. Build the Solution

```bash
dotnet build -p:Platform=x64
```

### 5. Run the Application

```bash
dotnet run --project src/UsbDeviceInspector/UsbDeviceInspector.csproj
```

### 6. Run Tests

```bash
dotnet test -p:Platform=x64
```

## Debugging in VS Code

This project is pre-configured for VS Code debugging:

1. **Open the project folder** in VS Code
2. **Set breakpoints** by clicking in the gutter next to line numbers
3. **Press F5** to start debugging (or use Run > Start Debugging)
4. The application will launch with the debugger attached

The debug configuration is defined in `.vscode/launch.json` and uses the CoreCLR debugger.

### Debug Tasks

VS Code tasks are pre-configured in `.vscode/tasks.json`:
- **Build** - Compiles the solution
- **Test** - Runs all unit tests
- **Run** - Launches the application

Access tasks via Terminal > Run Task or the Command Palette (Ctrl+Shift+P > "Tasks: Run Task").

## Local CI/CD Commands

These commands mirror the GitHub Actions workflow for local validation:

### Debug Build

```bash
dotnet build -p:Platform=x64
```

### Release Build

```bash
dotnet build -c Release -p:Platform=x64
```

### Run Tests

```bash
dotnet test -p:Platform=x64
```

### Full CI Validation (mirrors GitHub Actions)

```bash
dotnet restore UsbDeviceInspector.sln
dotnet build -c Release -p:Platform=x64 --no-restore
dotnet test -c Release -p:Platform=x64 --no-build
```

## Contributing

Contribution guidelines are coming soon. For now:

1. **Report bugs** via [GitHub Issues](https://github.com/dernerl/UsbDeviceInspector/issues)
2. **Suggest features** via [GitHub Issues](https://github.com/dernerl/UsbDeviceInspector/issues)
3. **Follow coding standards** defined in `docs/architecture/coding-standards.md`

## Troubleshooting

### Platform must be x64

**Symptom:** Build errors mentioning platform or architecture issues.

**Solution:** WinUI3 does not support AnyCPU. Always specify the platform:

```bash
dotnet build -p:Platform=x64
dotnet test -p:Platform=x64
```

### Windows SDK Version Mismatch

**Symptom:** Build errors about missing Windows SDK or target framework.

**Solution:** Ensure Windows 10 SDK (10.0.19041.0) is installed. This is typically included with the .NET 8 SDK. If missing:

1. Download the [Windows 10 SDK](https://developer.microsoft.com/en-us/windows/downloads/windows-sdk/)
2. Install version 10.0.19041.0 or later

### Windows App SDK Templates Not Found

**Symptom:** Cannot create new WinUI3 projects or template-related errors.

**Solution:** Install the templates:

```bash
dotnet new install Microsoft.WindowsAppSDK.Templates
```

### Build Fails with NuGet Restore Errors

**Symptom:** Package restore fails or packages cannot be found.

**Solution:**
1. Clear NuGet cache: `dotnet nuget locals all --clear`
2. Restore packages: `dotnet restore UsbDeviceInspector.sln`
3. Rebuild: `dotnet build -p:Platform=x64`

### VS Code IntelliSense Not Working

**Symptom:** No code completion, red squiggles everywhere, or "project not loaded".

**Solution:**
1. Ensure C# Dev Kit extension is installed and enabled
2. Wait for OmniSharp to initialize (check Output > C# panel)
3. If issues persist: Command Palette > ".NET: Restart Language Server"

## External Documentation

- **[WinUI3 Documentation](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/)** - UI framework reference
- **[.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)** - Runtime and SDK features
- **[Windows App SDK](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/)** - Windows APIs and packaging
- **[CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)** - MVVM pattern toolkit

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.