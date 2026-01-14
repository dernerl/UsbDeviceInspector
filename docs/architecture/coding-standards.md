# Coding Standards

**Purpose:** These standards are MANDATORY for AI agents and human developers. This section is intentionally minimal - assume developers know C# best practices. Focus is on project-specific conventions that prevent bugs and ensure consistency.

## Core Standards

**Languages & Runtimes:**
- C# 12.0 language features enabled in `.csproj` via `<LangVersion>12.0</LangVersion>`
- .NET 8.0 target framework (`<TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>`)
- Nullable reference types enabled project-wide (`<Nullable>enable</Nullable>`)
- Treat warnings as errors in Release builds (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in Release configuration)

**Style & Linting:**
- EditorConfig file (`.editorconfig`) defines formatting rules: 4-space indentation, `var` for obvious types, `this.` prefix discouraged
- Use .NET Compiler Platform (Roslyn) analyzers included in .NET 8 SDK (no additional StyleCop required)
- Async method suffix rule: All async methods MUST end with `Async` suffix (e.g., `EnumerateDevicesAsync`, not `EnumerateDevices`)
- Follow Microsoft's C# Coding Conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

**Test Organization:**
- Test files mirror source structure: `UsbDeviceInspector/Services/DeviceParsingService.cs` → `UsbDeviceInspector.Tests/Services/DeviceParsingServiceTests.cs`
- Test file naming convention: `{ClassUnderTest}Tests.cs` (e.g., `MainViewModelTests.cs`)
- Test method naming: `MethodName_Scenario_ExpectedBehavior` (e.g., `ParseDevices_WithMissingSerialNumber_ReturnsDeviceWithHasSerialNumberFalse`)

## Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Classes/Interfaces | PascalCase | `DeviceEnumerationService`, `IFormatService` |
| Methods | PascalCase | `EnumerateUsbStorageDevicesAsync()`, `ExtractVidPid()` |
| Properties | PascalCase | `VendorId`, `HasSerialNumber`, `IsLoading` |
| Private fields | `_camelCase` with underscore prefix | `_deviceEnumerationService`, `_logger` |
| Local variables | camelCase | `deviceInfo`, `enumResult`, `serialNumber` |
| Constants | PascalCase | `MaxRetryAttempts`, `EnumerationTimeout` |
| ViewModel commands | PascalCase ending in `Command` | `RefreshDevicesCommand`, `CopyForCrowdStrikeCommand` |
| XAML resources | PascalCase | `DeviceCardStyle`, `PrimaryButtonBrush` |

## Critical Rules

**1. Async/Await Usage:**
- All Windows API calls (device enumeration, clipboard) MUST use `async`/`await` - never block with `.Result` or `.Wait()`
- ViewModel command handlers that call async services MUST use `AsyncRelayCommand` from CommunityToolkit.Mvvm, not `RelayCommand`
- Rationale: Blocking async operations on UI thread causes deadlocks in WinUI3

**2. Dependency Injection:**
- Services MUST be injected via constructor parameters, never instantiated with `new` keyword in ViewModels or services
- Service interfaces MUST be defined in `Services/Interfaces/` directory and used for injection (e.g., inject `IDeviceEnumerationService`, not `DeviceEnumerationService`)
- Rationale: Enables unit testing with mocked dependencies, maintains single point of service registration

**3. Windows API Error Handling:**
- All Windows Runtime API calls MUST be wrapped in try-catch blocks with specific exception types (avoid generic `catch (Exception)`)
- Caught exceptions MUST be logged with full context (device ID, operation type, exception details)
- Rationale: Windows APIs throw cryptic COM exceptions; wrapping with context prevents debugging nightmares

**4. Observable Properties:**
- ViewModel properties bound to XAML MUST use `[ObservableProperty]` attribute from CommunityToolkit.Mvvm source generators
- Property change notifications are automatically generated - never manually call `OnPropertyChanged()` for attributed properties
- Rationale: Reduces boilerplate, prevents typos in property name strings, improves maintainability

**5. Resource Disposal:**
- Services implementing `IDisposable` (future logging file handles, etc.) MUST be registered as scoped or transient lifetime in DI container
- ViewModels MUST NOT implement `IDisposable` unless absolutely necessary (WinUI3 object lifetime is managed by framework)
- Rationale: Improper disposal in desktop apps causes memory leaks during long sessions

**6. XAML Data Binding:**
- Use `x:Bind` for compiled bindings with better performance, NOT `Binding` markup extension (traditional WPF-style binding)
- Specify binding mode explicitly: `x:Bind ViewModel.Devices, Mode=OneWay` (default is `OneTime` unlike WPF)
- Rationale: `x:Bind` provides compile-time validation and 2x faster performance than reflection-based `Binding`

**7. Device Instance Path Parsing:**
- All regex patterns MUST use `RegexOptions.Compiled` flag and be declared as static readonly fields (not inline in methods)
- Device Instance Path extraction MUST validate format before parsing - never assume well-formed input
- Rationale: Device Instance Paths vary by manufacturer; defensive parsing prevents crashes on unexpected hardware

**8. Clipboard Operations:**
- Clipboard API calls MUST include timeout and single retry logic (clipboard can be locked by DLP software)
- Never silently fail clipboard operations - always provide user feedback (success notification or error message)
- Rationale: Enterprise clipboard monitoring software causes intermittent failures; users need visibility into copy failures

**9. Logging Restrictions:**
- NEVER log full Device Instance Paths in production builds (may contain user-identifiable device serial numbers)
- Only log device counts, error counts, and anonymized error types
- Use Debug.WriteLine for development-only logging, Microsoft.Extensions.Logging for production
- Rationale: Privacy compliance and security (device serials could be considered PII in regulated industries)

**10. No Hardcoded Strings in UI:**
- All user-facing text MUST be declared as `x:Uid` resources in `.resw` files for future localization support
- Exception: Error messages embedded in service layer exceptions (these will be localized in Phase 2)
- Rationale: Enables internationalization without code changes (EU market expansion planned)
