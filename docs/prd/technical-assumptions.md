# Technical Assumptions

## Repository Structure: Monorepo

Single Git repository containing the WinUI3 application and unit tests:
```
UsbDeviceInspector/
├── src/
│   ├── UsbDeviceInspector/          # Main WinUI3 project
│   │   ├── Views/                    # XAML views (MainWindow.xaml)
│   │   ├── ViewModels/               # MVVM ViewModels
│   │   ├── Models/                   # Data models (UsbDevice class)
│   │   ├── Services/                 # Business logic (DeviceEnumerationService, ClipboardService)
│   │   └── Assets/                   # Icons, images
│   └── UsbDeviceInspector.Tests/    # Unit tests (xUnit)
├── .vscode/                          # VS Code configuration (launch.json, tasks.json)
├── docs/                             # Documentation (PRD, architecture, user guide)
├── .github/workflows/                # CI/CD pipeline definitions
└── README.md
```

**Rationale:** Monorepo simplifies MVP development with single-clone setup, unified CI/CD, and straightforward dependency management. No microservices or separate repositories needed for desktop utility.

## Architecture Overview

For detailed architecture diagrams, data flow, service responsibilities, and design patterns, see [architecture-diagram.md](architecture-diagram.md).

**Quick Overview:**
- **Architecture Type:** Monolithic Desktop Application with MVVM pattern
- **Layers:** Presentation (Views/ViewModels) → Business Logic (Services) → Data (In-Memory Models)
- **Key Patterns:** MVVM, Dependency Injection, Async/Await, Service Layer
- **Security Model:** Offline-only, zero telemetry, Standard-User compatible

## Service Architecture

**Monolithic desktop application** with service-oriented internal architecture:

- **Presentation Layer:** WinUI3 Views + ViewModels (MVVM pattern)
- **Business Logic Layer:** Service classes encapsulating domain logic
  - `DeviceEnumerationService`: Handles Windows.Devices.Enumeration API calls
  - `DeviceParsingService`: Extracts VID/PID/Serial from device properties
  - `ClipboardExportService`: Formats and copies data to clipboard
  - `FormatService`: Generates CrowdStrike and helpdesk output formats
- **Data Layer:** In-memory collections (no database, no persistence in MVP)

**Rationale:** Single-process desktop application doesn't require microservices or distributed architecture. Service classes provide testability and separation of concerns without over-engineering.

## Testing Requirements

**Unit + Integration testing approach:**

- **Unit Tests:** Core business logic in service classes (parsing, formatting, clipboard operations)
  - Target: 80%+ code coverage for service layer
  - Framework: xUnit with FluentAssertions
  - Mocking: NSubstitute for API dependencies
  - Test data: Mock device property collections covering edge cases (missing serial, malformed paths)

- **Integration Tests:** Device enumeration with Windows.Devices.Enumeration APIs
  - Real API calls against test USB devices (3-5 physical devices)
  - Validation of Device Instance Path parsing across manufacturers
  - Clipboard integration verification

- **Manual Testing:** UI/UX validation and end-to-end workflow testing
  - User acceptance testing with IT administrators (2-3 participants)
  - CrowdStrike Falcon Device Control format validation (paste into actual console)
  - Accessibility testing (keyboard navigation, screen reader compatibility)

**Out of Scope for MVP:**
- E2E UI automation tests (no Appium/WinAppDriver tests initially)
- Performance/load testing (manual validation sufficient for desktop utility)
- Automated accessibility testing (manual WCAG AA validation only)

**Rationale:** Unit + integration testing provides confidence in core functionality without over-investing in UI automation for MVP. Manual testing validates user experience and real-world compatibility.

## Development Environment

**IDE:** Visual Studio Code with C# Dev Kit extension (v1.0+)

**Required VS Code Extensions:**
- C# Dev Kit (Microsoft) - Project system, debugging, IntelliSense
- C# (Microsoft) - Base C# language support
- GitLens (optional but recommended) - Enhanced Git integration

**Build System:** .NET CLI (`dotnet build`, `dotnet run`, `dotnet test`)

**.NET Version:** .NET 8.0 SDK (LTS) - includes Windows 10 SDK

**WinUI3 Templates:** Install via:
```bash
dotnet new install Microsoft.WindowsAppSDK.Templates
```

**Project Creation:**
```bash
# Create solution and projects
dotnet new sln -n UsbDeviceInspector
dotnet new wasdk-winui-cs -n UsbDeviceInspector -o src/UsbDeviceInspector
dotnet new xunit -n UsbDeviceInspector.Tests -o src/UsbDeviceInspector.Tests
dotnet sln add src/UsbDeviceInspector/UsbDeviceInspector.csproj
dotnet sln add src/UsbDeviceInspector.Tests/UsbDeviceInspector.Tests.csproj
```

**Development Workflow:**
- XAML editing via text editor (syntax highlighting, no visual designer)
- Hot reload using `dotnet watch run` for rapid iteration
- Debugging via VS Code C# debugger (requires `.vscode/launch.json` configuration)
- Build/test automation via command line (identical to CI/CD pipeline)

**First-Time Setup:**
- Estimated 45 minutes to configure environment and launch.json
- Repository will include `.vscode/launch.json` and `.vscode/tasks.json` templates
- README will document step-by-step environment setup

**Package Manager:** NuGet (integrated with .NET CLI and VS Code NuGet UI)

**Rationale:** VS Code + .NET CLI provides lightweight development environment with excellent Git integration and scriptable builds. XAML hand-coding is acceptable for the simple card-based UI. Command-line workflow ensures CI/CD parity.

## Key Dependencies

**WinUI 3 & Windows App SDK:**
- Microsoft.WindowsAppSDK (version 1.5+)
- Microsoft.Windows.SDK.BuildTools

**MVVM Framework:**
- CommunityToolkit.Mvvm - INotifyPropertyChanged, RelayCommand, ObservableObject

**Testing:**
- xUnit - Test framework
- FluentAssertions - Assertion library
- NSubstitute - Mocking framework

**Icons:**
- Microsoft.UI.Xaml.Controls (Fluent icon set included in WinUI3)

**Rationale:** Minimal dependency footprint reduces attack surface and maintenance burden. CommunityToolkit.Mvvm is standard for WinUI3 MVVM development.

## Device Enumeration Strategy

**Primary API:** `Windows.Devices.Enumeration.DeviceInformation.FindAllAsync()`

**Device Selector:** `DeviceClass.PortableStorageDevice` or custom AQS (Advanced Query Syntax) filter to exclude internal SD card readers

**Async/Await Pattern:** All enumeration operations run asynchronously to prevent UI blocking

**Error Handling:** Graceful degradation when devices lack serial numbers or use non-standard Device Instance Path formats

**No WMI Fallback in MVP:** Device Instance Path parsing only; WMI queries deferred to Phase 2

**Rationale:** Windows.Devices.Enumeration provides Standard-User compatible device access without elevation. Async operations maintain UI responsiveness per NFR2.

## Clipboard Integration

**API:** `Windows.ApplicationModel.DataTransfer.DataPackage` for clipboard operations

**Format:** Plain text (no HTML or Rich Text formatting in MVP)

**Multi-Format Export:** Both CrowdStrike and Helpdesk formats copied as multi-line text in single clipboard operation

**Rationale:** Plain text clipboard integration is universal, requires no special permissions, and works across all enterprise applications (ticketing systems, web consoles).

## Packaging & Distribution

**MVP:** Unpackaged executable distributed via GitHub Releases (xcopy deployment)
- Framework-dependent deployment (requires .NET 8.0 Runtime on target machine)
- Zip file containing executable and required DLLs
- No installer required

**Post-MVP Considerations:**
- MSIX package for Microsoft Store distribution or enterprise deployment
- Self-contained deployment option (bundles .NET runtime, larger download)

**Code Signing:**
- Not required for MVP
- Consider post-validation for SmartScreen reputation and enterprise adoption

**Rationale:** Unpackaged deployment minimizes initial complexity. Framework-dependent deployment keeps download size small (<10MB). GitHub Releases provides simple distribution channel for community tools.

## Version Control & CI/CD

**Repository:** GitHub (public repository for open-source distribution)

**Branching Strategy:** GitHub Flow (main + feature branches, no separate develop branch)

**CI/CD:** GitHub Actions for automated builds and test execution
- **Trigger:** Push to main and pull requests
- **Actions:** `dotnet build`, `dotnet test`, artifact creation
- **Free unlimited minutes** for public repositories
- **Workflow file:** `.github/workflows/build.yml`

**Distribution:** GitHub Releases with binary zip files attached to tagged releases

**Rationale:** GitHub is the expected platform for IT admin community tools. GitHub Actions provides zero-cost CI/CD for public repos with same CLI commands used in development.

## Logging & Diagnostics

**MVP:** Console output during debugging only (`Debug.WriteLine`)

**Post-MVP:** Structured logging to local file (Serilog) for user troubleshooting without telemetry

**Rationale:** Minimal logging reduces complexity and privacy concerns. Debug output sufficient for development troubleshooting.

## Security & Privacy

**No Network Calls:** Application operates entirely offline (no API integrations in MVP)

**No Data Persistence:** No local database, no file storage beyond clipboard operations

**No Telemetry:** No usage analytics, crash reporting, or diagnostic data collection

**Standard-User Permissions:** Application must not require elevation at any point

**Rationale:** Offline-only and zero-telemetry design maximizes enterprise security compliance and user trust. Standard-User compatibility enables broad deployment without IT policy exceptions.

## Performance Optimization

**Async Operations:** All device enumeration and I/O operations use async/await pattern

**UI Virtualization:** ItemsRepeater with ItemsRepeaterScrollHost for efficient card rendering at scale

**Startup Optimization:** Lazy initialization of services to minimize cold start time (NFR6: <2 seconds)

**Rationale:** Async operations maintain UI responsiveness (NFR2). ItemsRepeater virtualization ensures performance with 10+ devices (NFR1: <3 seconds enumeration).

## Cross-Cutting Concerns

**Error Handling:**
- Global exception handler logs errors to Debug output
- Inline errors displayed on device cards for parsing failures
- Empty state messaging when no devices detected

**Input Validation:**
- Minimal validation needed (no user text input in MVP)
- Device property validation during parsing

**Localization:**
- English-only for MVP
- UI strings hardcoded (not resource files)
- Post-MVP: Consider localization if international demand exists

**Rationale:** Simplicity prioritized for MVP. English-only reduces complexity and testing surface. Error handling strategy aligns with graceful degradation requirements (FR14, FR15).

## Open Source License

**Recommendation:** MIT License

**Rationale:** Most permissive license, allows commercial use, minimal restrictions, maximizes community adoption and enterprise acceptance.
