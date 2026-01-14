# Epic 9: Testing & Quality Assurance

**Epic Goal:** Deliver comprehensive testing coverage across unit tests for business logic, integration tests for Windows API interactions, and manual testing for user experience validation. This epic validates MVP success criteria through systematic testing of service layer functionality, device enumeration accuracy with real hardware, CrowdStrike format correctness, and end-to-end workflow validation with IT administrators, ensuring the application meets functional requirements (FR1-FR15) and non-functional requirements (NFR1-NFR12) before distribution.

## Story 9.1: Achieve 80%+ Unit Test Coverage for Service Layer

As a **developer**,
I want **comprehensive unit tests for all service classes**,
so that **business logic is validated and regression-protected** (per Testing Requirements).

**Acceptance Criteria:**

1. Unit tests created for all service classes:
   - `DeviceEnumerationService`
   - `DeviceParsingService`
   - `FormatService`
   - `ClipboardExportService`
2. Test coverage measured using code coverage tool (dotnet-coverage or Coverlet)
3. Service layer test coverage achieves 80%+ (per Testing Requirements)
4. Tests use xUnit + FluentAssertions + NSubstitute
5. Tests cover:
   - Happy path scenarios (valid inputs, successful operations)
   - Edge cases (missing data, malformed inputs)
   - Error scenarios (exceptions, failures)
6. All tests pass consistently (no flaky tests)
7. Test execution time < 5 seconds for complete unit test suite
8. Coverage report generated and reviewed

## Story 9.2: Create Unit Tests for Device Parsing Logic

As a **developer**,
I want **comprehensive tests for VID/PID/Serial parsing**,
so that **parsing logic handles all known device format variations** (per NFR12: 90%+ parsing success).

**Acceptance Criteria:**

1. Unit tests created for `DeviceParsingService.ParseDeviceProperties()`
2. Test cases cover:
   - Standard Device Instance Path: `USB\VID_0781&PID_5581\4C530001231124103024`
   - Path with REV segment: `USB\VID_0781&PID_5581&REV_0100\SerialNumber`
   - Path without serial: `USB\VID_0781&PID_5581\` (should fail gracefully)
   - Lowercase VID/PID: `USB\vid_0781&pid_5581\Serial`
   - 3-digit hex VID/PID: `VID_078&PID_558` (edge case)
   - Missing VID or PID in HardwareIds
   - Multiple HardwareIds entries
3. Tests use FluentAssertions for readable assertions:
   - `device.IsValid.Should().BeTrue()`
   - `device.VendorId.Should().Be("0781")`
4. All test cases pass
5. Tests execute in < 100ms total (parsing performance validation)

## Story 9.3: Create Unit Tests for Format Generation

As a **developer**,
I want **tests validating CrowdStrike and Helpdesk format output**,
so that **generated formats match specifications exactly** (per NFR9: 100% CrowdStrike accuracy).

**Acceptance Criteria:**

1. Unit tests created for `FormatService.GenerateCrowdStrikeFormat()`
2. Test cases validate:
   - Combined ID format: `VID_0781&PID_5581\4C530001231124103024`
   - Manual Entry format with labeled fields
   - Both formats combined in single output
   - Uppercase hex formatting
   - Zero-padding for 3-digit hex values
3. Unit tests created for `FormatService.GenerateHelpdeskFormat()`
4. Test cases validate:
   - Human-readable format structure
   - All required fields included
   - Instructions footer included
   - Handling of missing fields (partial data)
5. Tests use string comparison with multi-line string literals
6. All format tests pass

## Story 9.4: Create Unit Tests for ViewModel Logic

As a **developer**,
I want **tests for ViewModel commands and state management**,
so that **UI logic is validated independently of actual UI rendering**.

**Acceptance Criteria:**

1. Unit tests created for `MainViewModel`
2. Test cases cover:
   - `LoadDevicesAsync()` populates Devices collection
   - `IsLoading` flag set correctly during enumeration
   - `IsEmpty` flag set when no devices found
   - `RefreshCommand` triggers device re-enumeration
   - `CopyCrowdStrikeCommand` calls export service
   - `CopyHelpdeskCommand` calls export service
   - `StatusMessage` updated correctly after operations
3. Tests use NSubstitute to mock service dependencies:
   - Mock `DeviceEnumerationService.EnumerateDevicesAsync()` to return test data
   - Mock export services to verify method calls
4. Tests validate observable property changes (PropertyChanged events)
5. All ViewModel tests pass
6. Tests execute in < 1 second total

## Story 9.5: Create Integration Tests for Device Enumeration

As a **developer**,
I want **integration tests using real Windows.Devices.Enumeration APIs**,
so that **device detection works with actual Windows APIs** (per Testing Requirements).

**Acceptance Criteria:**

1. Integration tests created in separate test class or project
2. Tests call real `DeviceInformation.FindAllAsync()` API (no mocking)
3. Test cases validate:
   - Device enumeration completes without exceptions
   - Returned devices include expected properties (FriendlyName, Manufacturer, etc.)
   - Device filtering excludes internal SD card readers
   - Enumeration completes within 3 seconds (NFR1)
4. Tests require at least 1 USB device connected (documented in test comments)
5. Tests marked with `[Fact(Skip = "Requires physical USB device")]` or similar (optional run)
6. Integration tests pass when run manually with USB device connected
7. Integration test results documented with device types tested

## Story 9.6: Validate Parsing Accuracy with Real USB Devices

As a **QA engineer**,
I want **parsing validated against real USB devices from major manufacturers**,
so that **90%+ parsing success rate is confirmed** (per NFR12).

**Acceptance Criteria:**

1. Manual testing performed with USB devices:
   - SanDisk (2+ models)
   - Kingston (2+ models)
   - Samsung (1+ model)
   - Generic/no-name brands (2+ devices)
   - Total: 10+ devices tested
2. For each device, validate:
   - VID parsed correctly
   - PID parsed correctly
   - Serial Number parsed correctly (if available)
   - IsValid flag set correctly
3. Parsing success rate calculated: (successful parses / total devices) >= 90%
4. Test results documented in `docs/device-compatibility-testing.md`:
   - Manufacturer | Model | VID | PID | Serial Parsed | IsValid | Notes
5. Failed parsing cases documented with Device Instance Path examples
6. Edge cases identified (devices without serial, unusual formats)
7. Success rate meets 90% threshold (per NFR12)

## Story 9.7: Validate CrowdStrike Format with Falcon Console

As a **QA engineer**,
I want **CrowdStrike format tested in actual Falcon Device Control console**,
so that **generated formats work in production environment** (per NFR9: 100% accuracy).

**Acceptance Criteria:**

1. CrowdStrike Falcon Device Control test environment accessed (or mock environment)
2. Manual testing workflow:
   - Launch USB Device Inspector
   - Detect USB device
   - Click "Copy for CrowdStrike"
   - Paste into CrowdStrike Falcon console (Combined ID field or manual entry fields)
   - Verify format accepted without errors
3. Test multiple devices (3+ devices) to validate format consistency
4. Test results documented in `docs/crowdstrike-format-validation.md`:
   - Device tested
   - Generated format
   - Paste result (accepted/rejected)
   - Any errors or issues
5. All tested formats accepted by CrowdStrike console (100% accuracy)
6. If format rejected, root cause identified and parsing/formatting logic fixed
7. Final validation confirms 100% acceptance rate

## Story 9.8: Conduct End-to-End User Acceptance Testing

As a **product manager**,
I want **end-to-end workflow tested with IT administrators**,
so that **user experience meets goals and workflow is efficient** (per Testing Requirements).

**Acceptance Criteria:**

1. User acceptance testing conducted with IT administrators (2-3 participants)
2. Test workflow:
   - Plug in USB device
   - Launch USB Device Inspector
   - Identify target device in card list
   - Click "Copy for CrowdStrike"
   - Paste into CrowdStrike console (or mock system)
   - Whitelist device
   - Measure time from device connection to whitelisting completion
3. Success criteria validated:
   - Workflow completes in < 60 seconds (per Goals: reduce from 5-10 minutes to <60 seconds)
   - Zero training required (users complete workflow without documentation)
   - No errors or confusion during workflow
4. User feedback collected on:
   - UI clarity and scannability
   - Button labels and actions
   - Error messages (if encountered)
   - Overall satisfaction
5. Feedback documented in `docs/user-acceptance-testing.md`
6. Critical issues identified and fixed before release
7. Success criteria met (workflow time, zero training, no major issues)

## Story 9.9: Test Performance and Responsiveness

As a **QA engineer**,
I want **performance validated against non-functional requirements**,
so that **application meets speed and responsiveness targets** (per NFR1, NFR2, NFR6).

**Acceptance Criteria:**

1. Performance testing conducted for:
   - **Cold start time:** Application launch to window displayed (NFR6: <2 seconds)
   - **Device enumeration time:** Time to enumerate 10 devices (NFR1: <3 seconds)
   - **UI responsiveness:** UI remains interactive during enumeration (NFR2)
   - **Memory footprint:** Memory usage during typical operation (NFR5: <100MB)
2. Testing performed on representative hardware:
   - Windows 10/11 machine with SSD and 8GB+ RAM
   - 10 USB devices connected simultaneously
3. Test results documented with measurements:
   - Cold start time: X.XX seconds
   - Enumeration time (10 devices): X.XX seconds
   - Memory footprint: XX MB
4. All performance targets met (or documented as known limitations)
5. UI responsiveness validated manually:
   - Window remains draggable during enumeration
   - No "Not Responding" states in Task Manager
6. Performance bottlenecks identified and optimized if targets not met

## Story 9.10: Create Regression Test Suite

As a **developer**,
I want **a comprehensive regression test suite**,
so that **future changes don't break existing functionality**.

**Acceptance Criteria:**

1. Regression test suite includes:
   - All unit tests (service layer, ViewModels)
   - Integration tests (device enumeration)
   - Format validation tests
   - Error handling tests
2. Test suite runs via single command: `dotnet test`
3. All tests pass consistently (no flaky tests)
4. Test execution time < 10 seconds for complete suite (unit + integration)
5. Tests integrated into CI/CD pipeline (GitHub Actions)
6. Pull requests require all tests passing before merge
7. Test results displayed in GitHub Actions workflow output
