# Epic 2: USB Device Enumeration & Detection

**Epic Goal:** Implement the core device detection functionality using Windows.Devices.Enumeration API to discover and enumerate all connected USB storage devices from Standard-User accounts without administrator privileges. This epic delivers the foundational capability to detect USB devices asynchronously while maintaining UI responsiveness, filtering out non-USB storage devices, and providing users with manual refresh capabilities to detect newly connected devices.

## Story 2.1: Implement DeviceEnumerationService with Basic USB Detection

As a **developer**,
I want **a service class that uses Windows.Devices.Enumeration API to detect USB storage devices**,
so that **the application can discover connected devices without requiring elevation**.

**Acceptance Criteria:**

1. `DeviceEnumerationService` class created in `Services/` folder
2. Service implements async method `EnumerateDevicesAsync()` that returns `IEnumerable<DeviceInformation>`
3. Method uses `DeviceInformation.FindAllAsync()` with device selector targeting portable storage devices
4. Service operates successfully on Standard-User Windows accounts (validated manually without UAC prompts)
5. Service includes XML documentation comments for public methods
6. Unit tests created validating service instantiation and method signatures
7. No administrator privileges required or elevation prompts triggered during enumeration

## Story 2.2: Implement Device Filtering for USB Storage Only

As a **developer**,
I want **device enumeration to filter out internal SD card readers and non-USB storage devices**,
so that **users only see relevant removable USB storage devices** (per FR5).

**Acceptance Criteria:**

1. Device selector uses Advanced Query Syntax (AQS) to filter for USB interface devices
2. Filtering logic excludes devices with SystemDeviceID containing "SD" or "MMC" patterns
3. Filtering logic verifies device connection type is USB (not internal SATA/PCIe)
4. Service method returns only USB storage devices (flash drives, external HDDs, USB card readers)
5. Unit tests validate filtering logic with mock device collections containing:
   - USB flash drives (should be included)
   - Internal SD card readers (should be excluded)
   - USB-connected card readers (should be included)
   - Internal SSDs (should be excluded)
6. Manual testing with 3+ physical device types validates filtering behavior
7. Console logging (Debug output) shows filtered device count for troubleshooting

## Story 2.3: Implement Async/Await Pattern for UI Responsiveness

As a **developer**,
I want **all device enumeration operations to execute asynchronously on background threads**,
so that **the UI remains responsive during device detection** (per NFR2).

**Acceptance Criteria:**

1. `EnumerateDevicesAsync()` method returns `Task<IEnumerable<DeviceInformation>>`
2. Method uses `await` keyword for all Windows.Devices.Enumeration API calls
3. Method runs entirely on background thread pool (no UI thread blocking)
4. Calling code uses `ConfigureAwait(false)` to avoid UI context marshalling where applicable
5. Unit tests validate async behavior using `Task.Run()` and completion verification
6. Manual testing confirms UI window remains draggable and responsive during 10-device enumeration
7. No UI freezing or "Not Responding" window states during enumeration (validated with Task Manager)
8. Enumeration completes within 3 seconds for up to 10 devices (NFR1 validation)

## Story 2.4: Add Refresh Functionality to Re-Enumerate Devices

As an **IT administrator**,
I want **a manual refresh capability to detect newly connected USB devices**,
so that **I can plug in a device and refresh without restarting the application** (per FR11).

**Acceptance Criteria:**

1. `DeviceEnumerationService` provides `RefreshDevicesAsync()` method that re-runs enumeration
2. Method clears any previous device cache/state before re-enumerating
3. Method returns updated device collection after refresh operation
4. Unit tests validate refresh behavior returns new device list on each invocation
5. Service includes refresh timestamp property (`LastRefreshTime`) updated after each successful refresh
6. Debug logging indicates refresh initiation and completion with device count
7. Manual testing validates:
   - Initial launch enumerates devices
   - Plug in new USB device
   - Trigger refresh
   - New device appears in results

## Story 2.5: Implement Automatic Device Enumeration on Application Launch

As an **IT administrator**,
I want **the application to automatically detect USB devices when launched**,
so that **I immediately see connected devices without manual action** (per FR12).

**Acceptance Criteria:**

1. Application startup logic calls `EnumerateDevicesAsync()` automatically during initialization
2. Enumeration occurs after UI window is rendered (doesn't block window display)
3. Application window displays within 2 seconds of launch (cold start time per NFR6)
4. Device enumeration completes within 3 seconds after window appears (per NFR1)
5. No user interaction required to trigger initial enumeration
6. Manual testing validates:
   - Launch application with USB device connected
   - Devices appear automatically within 5 seconds total (2s window + 3s enumeration)
7. Debug logging indicates automatic enumeration start and completion

## Story 2.6: Handle DeviceInformation Properties Collection

As a **developer**,
I want **to access and parse DeviceInformation.Properties collection**,
so that **I can extract detailed device metadata for parsing in Epic 3**.

**Acceptance Criteria:**

1. `DeviceEnumerationService` returns full `DeviceInformation` objects (not filtered/transformed)
2. Service requests additional properties in device selector using property keys:
   - `System.ItemNameDisplay` (friendly name)
   - `System.Devices.Manufacturer` (manufacturer)
   - `System.Devices.HardwareIds` (HardwareIds array)
   - `System.Devices.DeviceInstanceId` (Device Instance Path)
3. Properties collection accessible via `DeviceInformation.Properties` dictionary
4. Unit tests validate property collection contains expected keys using mock data
5. Service includes helper method `GetPropertyValue<T>(DeviceInformation, string key)` with null-safety
6. Debug logging outputs property count for each enumerated device
7. Manual testing validates properties populated for real USB devices (not null/empty)

## Story 2.7: Create UsbDevice Model Class

As a **developer**,
I want **a data model class representing a USB device with all relevant properties**,
so that **I have a strongly-typed object to pass between layers** (separation from DeviceInformation).

**Acceptance Criteria:**

1. `UsbDevice` class created in `Models/` folder
2. Class includes properties:
   - `string Id` (unique device identifier)
   - `string FriendlyName`
   - `string Manufacturer`
   - `string VendorId` (placeholder, populated in Epic 3)
   - `string ProductId` (placeholder, populated in Epic 3)
   - `string SerialNumber` (placeholder, populated in Epic 3)
   - `string DeviceInstancePath` (raw Device Instance Path)
   - `bool IsValid` (indicates successful parsing, populated in Epic 3)
   - `string ErrorMessage` (parsing error details, populated in Epic 8)
3. Class implements `INotifyPropertyChanged` for UI binding (using CommunityToolkit.Mvvm `ObservableObject`)
4. Class includes constructor accepting `DeviceInformation` parameter
5. Constructor extracts basic properties (FriendlyName, Manufacturer, DeviceInstancePath) from DeviceInformation
6. Unit tests validate model construction from mock `DeviceInformation` objects
7. Model class includes XML documentation for all public members

## Story 2.8: Implement Service-to-Model Mapping

As a **developer**,
I want **DeviceEnumerationService to return UsbDevice objects instead of DeviceInformation**,
so that **the rest of the application works with domain models rather than platform APIs**.

**Acceptance Criteria:**

1. `EnumerateDevicesAsync()` method signature returns `Task<IEnumerable<UsbDevice>>`
2. Service internally converts each `DeviceInformation` to `UsbDevice` using model constructor
3. Mapping logic handles missing or null DeviceInformation properties gracefully (no exceptions)
4. Service includes error handling for mapping failures (logs errors, continues with remaining devices)
5. Unit tests validate:
   - Successful mapping of valid DeviceInformation to UsbDevice
   - Graceful handling of DeviceInformation with missing properties
   - Empty collection returned when no devices found (not null)
6. Service returns empty list (not null) when enumeration finds zero devices
7. Debug logging indicates number of devices successfully mapped vs. total enumerated
