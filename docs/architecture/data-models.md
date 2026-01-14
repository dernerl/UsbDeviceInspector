# Data Models

## UsbDevice

**Purpose:** Represents a single USB storage device detected by the system, encapsulating all device identifiers and properties needed for CrowdStrike and helpdesk export formats.

**Key Attributes:**
- `Id`: `string` - Unique identifier for the device (derived from DeviceInformation.Id), used as primary key for in-memory collection
- `FriendlyName`: `string` - Human-readable device name (e.g., "SanDisk Ultra USB 3.0"), displayed prominently on device cards
- `Manufacturer`: `string?` - Device manufacturer name (nullable - some devices don't expose this property)
- `VendorId`: `string` - Vendor ID in format "0781" (4-character hex string), extracted from HardwareIds or Device Instance Path
- `ProductId`: `string` - Product ID in format "5581" (4-character hex string), extracted from HardwareIds or Device Instance Path
- `SerialNumber`: `string?` - Device serial number (nullable - critical for CrowdStrike format but not all devices expose it), extracted from Device Instance Path pattern `USB\VID_xxxx&PID_xxxx\SerialNumber`
- `DeviceInstancePath`: `string` - Full Windows Device Instance Path (e.g., `USB\VID_0781&PID_5581\4C530001231120115142`), used for parsing and debugging
- `HasSerialNumber`: `bool` - Computed property indicating whether SerialNumber is available, controls UI state (show error message if false)
- `IsValid`: `bool` - Computed property indicating whether device has minimum required properties (VendorId, ProductId), used to filter invalid devices

**Relationships:**
- **No relationships** - This is a flat model with no navigation properties. The application maintains a simple `ObservableCollection<UsbDevice>` in the ViewModel for UI binding.
- **Future consideration:** If Phase 2 adds device history or favorites, might introduce `UsbDeviceHistory` entity with one-to-many relationship.

## DeviceEnumerationResult

**Purpose:** Wrapper model returned by `DeviceEnumerationService` containing enumeration results and any errors encountered during device detection.

**Key Attributes:**
- `Devices`: `List<UsbDevice>` - Collection of successfully parsed USB devices
- `Errors`: `List<DeviceParsingError>` - Collection of devices that failed parsing with error details
- `EnumerationTime`: `TimeSpan` - Time taken to complete enumeration, used for performance monitoring (NFR1: <3 seconds validation)
- `TotalDevicesFound`: `int` - Total count of devices discovered before filtering, used for diagnostics

**Relationships:**
- Contains `List<UsbDevice>` (composition relationship)
- Contains `List<DeviceParsingError>` (composition relationship)

## DeviceParsingError

**Purpose:** Captures parsing failures for individual devices, enabling inline error display on device cards.

**Key Attributes:**
- `DeviceId`: `string` - Device ID from Windows API for correlation
- `DeviceName`: `string` - Friendly name if available, or "Unknown Device"
- `ErrorType`: `DeviceErrorType` (enum) - Category of error (`MissingSerialNumber`, `InvalidDeviceInstancePath`, `MissingVidPid`, `InternalSdCardReader`)
- `ErrorMessage`: `string` - User-friendly error description (e.g., "This device does not expose a serial number and cannot be used with CrowdStrike Device Control")
- `DeviceInstancePath`: `string?` - Raw Device Instance Path for debugging (may be null if parsing failed early)

**Relationships:**
- No relationships - Simple error DTO

## DeviceErrorType (Enum)

**Purpose:** Categorizes device parsing failures for consistent error handling and reporting.

**Values:**
- `MissingSerialNumber` - Device has VID/PID but no serial number
- `InvalidDeviceInstancePath` - Device Instance Path doesn't match expected USB pattern
- `MissingVidPid` - Cannot extract VID or PID from device properties
- `InternalSdCardReader` - Device is internal SD card reader (excluded by filter)
- `UnknownError` - Catch-all for unexpected parsing failures
