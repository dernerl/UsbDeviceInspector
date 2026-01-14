# Test Strategy and Standards

## Testing Philosophy

**Approach:** Test-after development with comprehensive unit test coverage for service layer. UI testing deferred to manual QA until post-MVP (WinUI3 UI automation tooling is immature).

**Coverage Goals:**
- Service layer (business logic): 80%+ line coverage target
- ViewModel layer: 70%+ coverage (focused on command logic and state management)
- UI layer (XAML code-behind): Manual testing only for MVP (typically minimal code with x:Bind)
- Overall project: 75%+ coverage target excluding UI layer

**Test Pyramid:**
- **80% Unit Tests:** Fast, isolated tests for service methods and ViewModels with mocked dependencies
- **15% Integration Tests:** Service layer tests against real Windows API (requires USB device connected during CI)
- **5% Manual E2E Tests:** Human QA workflow validation (install MSIX, plug device, verify copy functionality)

**Rationale:** Service layer contains all business logic (parsing, formatting, Windows API interaction). High coverage here ensures correctness. WinUI3 UI testing tools are unreliable, so manual QA provides better ROI for desktop apps.

## Test Types and Organization

### Unit Tests

**Framework:** xUnit 2.6.6 - Industry standard for .NET with excellent Visual Studio and VS Code integration

**File Convention:** `{ClassUnderTest}Tests.cs` (e.g., `DeviceParsingServiceTests.cs`, `MainViewModelTests.cs`)

**Location:** `src/UsbDeviceInspector.Tests/` directory, mirroring source structure
- `Services/DeviceParsingServiceTests.cs` mirrors `UsbDeviceInspector/Services/DeviceParsingService.cs`
- `ViewModels/MainViewModelTests.cs` mirrors `UsbDeviceInspector/ViewModels/MainViewModel.cs`

**Mocking Library:** NSubstitute 5.1.0 - Simplest syntax in .NET ecosystem for creating test doubles

**Coverage Requirement:** 80%+ for service layer, 70%+ for ViewModels

**AI Agent Requirements for Unit Tests:**
- Generate tests for ALL public methods in service classes
- Cover happy path AND edge cases (null inputs, empty collections, invalid formats)
- Cover error conditions (exceptions thrown, timeout scenarios, retry logic)
- Follow AAA pattern (Arrange, Act, Assert) with clear section comments
- Mock all external dependencies (Windows APIs, other services, loggers)
- Use descriptive test method names: `MethodName_Scenario_ExpectedBehavior`

**Example Test Structure:**
```csharp
[Fact]
public void ExtractVidPid_ValidDeviceInstancePath_ReturnsCorrectVidAndPid()
{
    // Arrange
    var service = new DeviceParsingService();
    var deviceInstancePath = @"USB\VID_0781&PID_5581\4C530001231120115142";

    // Act
    var result = service.ExtractVidPid(deviceInstancePath);

    // Assert
    result.Should().NotBeNull();
    result.Value.VendorId.Should().Be("0781");
    result.Value.ProductId.Should().Be("5581");
}
```

**Key Testing Scenarios:**
- `DeviceParsingService`: Test VID/PID extraction with various Device Instance Path formats (USB 2.0, USB 3.0, USB-C), test serial number extraction, test missing serial handling, test invalid path formats
- `FormatService`: Test CrowdStrike format generation, test helpdesk format with/without manufacturer, test bulk export with multiple devices
- `MainViewModel`: Test device collection updates after enumeration, test command enablement logic (copy button disabled when no serial), test loading state transitions

### Integration Tests

**Scope:** Verify service layer integration with real Windows APIs. These tests run on CI agents with USB devices attached.

**Location:** `src/UsbDeviceInspector.Tests/Integration/` subdirectory

**Test Infrastructure:**
- **Windows API:** Real `Windows.Devices.Enumeration` API calls (no mocks) - requires USB device connected to CI runner
- **Clipboard:** Mocked via abstraction layer (clipboard testing on CI is unreliable due to RDP session limitations)
- **Test Devices:** CI pipeline has dedicated USB flash drive permanently attached to GitHub Actions runner (Windows 2022 image with physical hardware)

**Integration Test Scenarios:**
- `DeviceEnumerationIntegrationTests`: Verify real device enumeration returns expected properties, verify Device Instance Path format matches Windows conventions, verify enumeration completes in <3 seconds per NFR1
- `FullWorkflowIntegrationTests`: End-to-end test: enumerate → parse → format → (mocked) clipboard copy

**CI Configuration:**
- Integration tests tagged with `[Trait("Category", "Integration")]` attribute
- Run after unit tests in separate GitHub Actions job
- Allowed to fail without blocking PR merge (hardware dependency makes tests flaky)

### End-to-End Tests

**Framework:** Manual QA only for MVP. Post-MVP Phase 2 may introduce WinAppDriver or Appium for WinUI3 automation.

**Scope:** Full application workflows from user perspective:
1. Install MSIX package on clean Windows VM
2. Launch application, verify automatic device enumeration
3. Verify device cards display with correct information
4. Test "Copy for CrowdStrike" button, verify clipboard contains expected format
5. Test "Copy for Helpdesk" button, verify alternative format
6. Plug in second USB device, click Refresh, verify new device appears
7. Verify device without serial number shows error message inline

**Environment:** Manual testing performed on:
- Windows 10 21H2 VM (minimum supported version)
- Windows 11 23H2 VM (latest release)
- Standard User account (no admin rights) to validate NFR9 requirement

**Test Data:** Physical USB devices used for testing:
- SanDisk Ultra USB 3.0 (known serial number, used as reference device)
- Generic no-name USB 2.0 drive (no serial number exposed, tests error handling)
- Built-in SD card reader (should be filtered out by enumeration logic)

## Test Data Management

**Strategy:** Embedded test data fixtures in test project for unit tests. Integration tests use real connected hardware.

**Fixtures:** `src/UsbDeviceInspector.Tests/TestData/` directory contains:
- `SampleDevicePaths.cs`: Static class with 20+ real Device Instance Path strings collected from various USB devices
- `MockDeviceInformation.cs`: Factory methods for creating `DeviceInformation` mocks with NSubstitute

**Factories:** Use builder pattern for complex test objects:
```csharp
var device = new UsbDeviceBuilder()
    .WithVendorId("0781")
    .WithProductId("5581")
    .WithSerialNumber("4C530001231120115142")
    .Build();
```

**Cleanup:** Not required - all tests are stateless with no database or file system modifications. Device enumeration is read-only.

## Continuous Testing

**CI Integration:**
- **On PR Creation:** Run full unit test suite with code coverage report (blocks merge if coverage drops below 75%)
- **On Commit to main:** Run unit + integration tests, publish coverage to Codecov
- **On Tag Creation (Release):** Run full test suite including extended integration tests, manual QA sign-off required before publishing MSIX

**Performance Tests:** Not implemented for MVP. Phase 2 may add BenchmarkDotNet tests for device parsing performance (target: parse 100 devices in <100ms).

**Security Tests:** Not implemented for MVP. Phase 2 will add:
- Dependency scanning with GitHub Dependabot (automated PR creation for vulnerable packages)
- SAST scanning with SonarCloud (code quality and security vulnerability detection)
- Manual security review before Microsoft Store publication
