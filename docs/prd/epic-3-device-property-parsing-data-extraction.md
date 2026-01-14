# Epic 3: Device Property Parsing & Data Extraction

**Epic Goal:** Implement robust parsing logic to extract Vendor ID (VID), Product ID (PID), and Serial Number from Windows device properties, specifically from HardwareIds arrays and Device Instance Path strings. This epic delivers the critical data extraction capabilities needed to generate CrowdStrike-compatible identifiers, handling edge cases like missing serial numbers and malformed paths while maintaining parsing success rates above 90% for major USB manufacturers.

## Story 3.1: Create DeviceParsingService for Property Extraction

As a **developer**,
I want **a dedicated service class responsible for parsing device properties**,
so that **parsing logic is separated from enumeration and can be independently tested**.

**Acceptance Criteria:**

1. `DeviceParsingService` class created in `Services/` folder
2. Service includes method `ParseDeviceProperties(UsbDevice device)` that populates VID/PID/Serial
3. Method accepts `UsbDevice` reference and mutates properties (in-place parsing)
4. Method returns `bool` indicating parsing success or failure
5. Service is stateless (no instance fields, pure parsing logic)
6. Service includes XML documentation for all public methods
7. Unit tests validate service instantiation and method signatures
8. Service constructor accepts no dependencies (utility/helper service)

## Story 3.2: Parse Vendor ID and Product ID from HardwareIds Array

As a **developer**,
I want **to extract VID and PID from the HardwareIds property array**,
so that **I can identify device manufacturer and model** (per FR3).

**Acceptance Criteria:**

1. Parsing logic extracts VID from HardwareIds string matching pattern `VID_xxxx` (4 hex digits)
2. Parsing logic extracts PID from HardwareIds string matching pattern `PID_xxxx` (4 hex digits)
3. VID and PID extracted in hexadecimal format (e.g., `0781`, `5581`)
4. Logic handles HardwareIds array with multiple entries (searches all entries until match found)
5. Logic handles case-insensitive matching (`VID_`, `Vid_`, `vid_` all supported)
6. Parsing sets `UsbDevice.VendorId` and `UsbDevice.ProductId` properties
7. Unit tests validate parsing with HardwareIds arrays containing:
   - Standard format: `USB\VID_0781&PID_5581&REV_0100`
   - Multiple entries with VID/PID in different positions
   - Lowercase patterns
   - Missing VID or PID (negative test - should fail gracefully)
8. Manual testing validates parsing with real USB devices from SanDisk, Kingston, Samsung

## Story 3.3: Parse Serial Number from Device Instance Path

As a **developer**,
I want **to extract Serial Number from Device Instance Path**,
so that **I can uniquely identify individual device units** (per FR4).

**Acceptance Criteria:**

1. Parsing logic extracts Serial Number from Device Instance Path pattern: `USB\VID_xxxx&PID_xxxx\SerialNumber`
2. Serial Number is the substring after the second backslash (`\`) in the path
3. Logic handles Device Instance Paths with varying formats:
   - Standard: `USB\VID_0781&PID_5581\4C530001231124103024`
   - With revision: `USB\VID_0781&PID_5581&REV_0100\4C530001231124103024`
   - Multiple segments (uses last segment as serial)
4. Parsing sets `UsbDevice.SerialNumber` property
5. Parsing handles missing Serial Number gracefully (sets property to null, sets IsValid = false)
6. Unit tests validate parsing with Device Instance Paths:
   - Standard format with serial
   - Format with REV segment
   - Path with missing serial (e.g., `USB\VID_0781&PID_5581\` with no serial)
   - Path with ampersand-separated segments after VID/PID
7. Parsing trims whitespace from extracted Serial Number
8. Manual testing validates parsing with 5+ real USB devices from different manufacturers

## Story 3.4: Implement Regex-Based Parsing for Robustness

As a **developer**,
I want **parsing logic to use regular expressions for pattern matching**,
so that **parsing handles format variations and edge cases robustly**.

**Acceptance Criteria:**

1. VID parsing uses regex: `VID_([0-9A-Fa-f]{4})` to extract 4-digit hex value
2. PID parsing uses regex: `PID_([0-9A-Fa-f]{4})` to extract 4-digit hex value
3. Serial Number parsing uses regex or string split logic to extract substring after last `\` in path
4. Regex matching uses `RegexOptions.IgnoreCase` for case-insensitive matching
5. Parsing handles invalid Device Instance Paths without throwing exceptions
6. Unit tests validate regex behavior with edge cases:
   - Extra whitespace around patterns
   - Non-hex characters in VID/PID positions (should fail to match)
   - Device Instance Paths with unexpected formats (should return IsValid = false)
7. Performance: Regex parsing completes in <10ms per device (validated with Stopwatch timing in tests)

## Story 3.5: Set IsValid Flag and Error Messages

As a **developer**,
I want **parsing to set IsValid flag and ErrorMessage property**,
so that **the UI can display inline errors for devices that fail parsing** (per FR14).

**Acceptance Criteria:**

1. `DeviceParsingService.ParseDeviceProperties()` sets `UsbDevice.IsValid = true` when all required properties extracted
2. Method sets `UsbDevice.IsValid = false` when VID, PID, or Serial Number parsing fails
3. Method sets `UsbDevice.ErrorMessage` with descriptive error when parsing fails:
   - "Serial Number unavailable" when serial missing
   - "Unable to parse device identifiers" when VID/PID parsing fails
   - "Device Instance Path format not recognized" when path format is unexpected
4. Error messages are user-friendly (no stack traces or technical jargon)
5. Unit tests validate error message content for each failure scenario
6. Devices with IsValid = false still populate FriendlyName and Manufacturer (partial data shown)
7. Service method returns `false` when IsValid is set to false

## Story 3.6: Integrate Parsing into DeviceEnumerationService

As a **developer**,
I want **device enumeration to automatically parse properties using DeviceParsingService**,
so that **UsbDevice objects returned from enumeration are fully populated**.

**Acceptance Criteria:**

1. `DeviceEnumerationService` instantiates `DeviceParsingService` in constructor
2. After creating `UsbDevice` from `DeviceInformation`, service calls `ParseDeviceProperties(device)`
3. Parsing occurs for every enumerated device before returning collection
4. Enumeration continues even if parsing fails for individual devices (no exceptions thrown)
5. Debug logging indicates parsing success/failure count (e.g., "Parsed 3 of 4 devices successfully")
6. Unit tests validate integration:
   - Enumeration returns devices with populated VID/PID/Serial
   - Devices with parsing failures have IsValid = false
7. Manual testing validates end-to-end flow:
   - Launch application
   - Plug in USB device
   - Device appears with VID, PID, Serial Number populated

## Story 3.7: Validate Parsing Accuracy with Real Devices

As a **QA engineer**,
I want **parsing logic validated against USB devices from major manufacturers**,
so that **we achieve 90%+ parsing success rate** (per NFR12).

**Acceptance Criteria:**

1. Manual testing conducted with USB devices from:
   - SanDisk (2+ models)
   - Kingston (2+ models)
   - Samsung (1+ model)
   - Generic/no-name USB drives (2+ devices)
2. Parsing success rate calculated: (successful parses / total devices tested) * 100
3. Success rate meets or exceeds 90% threshold
4. Failed parsing cases documented with Device Instance Path examples for future improvement
5. Test results documented in `docs/device-compatibility-testing.md` with table format:
   - Manufacturer | Model | VID | PID | Serial Parsed | Notes
6. Edge cases identified and documented (e.g., devices without serial numbers)
7. Parsing performance measured: average time per device < 50ms

## Story 3.8: Handle Devices Without Serial Numbers Gracefully

As an **IT administrator**,
I want **devices without serial numbers to be clearly identified with error messages**,
so that **I understand why CrowdStrike format cannot be generated** (per FR14).

**Acceptance Criteria:**

1. Devices with missing Serial Number have `IsValid = false`
2. ErrorMessage property set to "Serial Number unavailable - this device cannot be whitelisted by serial"
3. Device card in UI displays error message (validated after Epic 6 UI implementation)
4. VID and PID still displayed for devices without serial numbers
5. "Copy for CrowdStrike" button disabled or shows alternate message for invalid devices
6. Unit tests validate error handling for devices without serial numbers
7. Manual testing with device lacking serial number confirms graceful degradation
8. Debug logging indicates devices without serial numbers detected during parsing
