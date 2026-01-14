# Epic 8: Error Handling & Edge Cases

**Epic Goal:** Implement comprehensive error handling to gracefully manage edge cases including device parsing failures, missing serial numbers, clipboard operation failures, enumeration errors, and the scenario where no USB devices are connected. This epic delivers robustness and user confidence through clear error messaging, graceful degradation, and inline error displays, ensuring users understand what went wrong and how to proceed even when operations fail.

## Story 8.1: Implement Global Exception Handler

As a **developer**,
I want **a global exception handler to catch unhandled exceptions**,
so that **the application doesn't crash unexpectedly** (per Cross-Cutting Concerns).

**Acceptance Criteria:**

1. Global exception handler registered in `App.xaml.cs`:
   - `UnhandledException` event handler for application-level exceptions
   - `TaskScheduler.UnobservedTaskException` event handler for async exceptions
2. Exception handler logs error details to Debug output:
   - Exception type
   - Exception message
   - Stack trace
3. Exception handler displays user-friendly error message dialog (not technical stack trace)
4. Exception handler marks exception as handled to prevent app crash (where appropriate)
5. Unit tests validate exception handler registration (integration tests for actual handling)
6. Manual testing: trigger unhandled exception, verify app displays error dialog without crashing

## Story 8.2: Handle Device Enumeration Failures

As a **user**,
I want **clear error messages when device enumeration fails**,
so that **I understand the issue and can take corrective action**.

**Acceptance Criteria:**

1. `DeviceEnumerationService.EnumerateDevicesAsync()` includes try-catch for enumeration exceptions
2. Service catches specific exceptions:
   - `UnauthorizedAccessException` - insufficient permissions
   - `COMException` - Windows API failures
   - Generic exceptions
3. Service logs exception details to Debug output
4. Service returns empty device list on failure (not null, no exception thrown to caller)
5. ViewModel detects enumeration failure and sets StatusMessage:
   - "Unable to access USB devices. Try running the application again."
6. Error message non-technical and actionable
7. Unit tests validate exception handling for each exception type
8. Manual testing: simulate enumeration failure, verify error message displayed

## Story 8.3: Handle Missing Serial Numbers with Inline Errors

As a **user**,
I want **devices without serial numbers clearly identified with explanations**,
so that **I understand why I cannot whitelist these devices** (per FR14).

**Acceptance Criteria:**

1. Devices with missing Serial Number have `IsValid = false` (implemented in Epic 3)
2. Error banner on device card displays message: "Serial Number unavailable - this device cannot be whitelisted by serial"
3. Copy for CrowdStrike button disabled with tooltip: "Cannot generate CrowdStrike format without serial number"
4. Copy for Helpdesk button enabled (partial device info can still be shared)
5. Error banner styling uses warning colors (yellow/orange, not aggressive red)
6. Unit tests validate error display for devices without serial numbers
7. Manual testing with USB device lacking serial number confirms:
   - Error banner visible
   - CrowdStrike button disabled
   - Helpdesk button enabled
   - Error message clear and helpful

## Story 8.4: Handle Parsing Failures for VID/PID

As a **user**,
I want **devices with VID/PID parsing failures clearly identified**,
so that **I know these devices cannot be whitelisted**.

**Acceptance Criteria:**

1. Devices with VID/PID parsing failures have `IsValid = false`
2. Error message set to: "Unable to parse device identifiers"
3. Error banner displays on device card
4. Both Copy for CrowdStrike and Copy for Helpdesk buttons disabled (cannot generate any format)
5. Device card still displays FriendlyName and Manufacturer if available (partial data)
6. Unit tests validate error handling for VID/PID parsing failures
7. Manual testing: create mock device with malformed HardwareIds, verify error display

## Story 8.5: Handle Clipboard Operation Failures

As a **user**,
I want **clear feedback when clipboard operations fail**,
so that **I know to retry or use an alternative method**.

**Acceptance Criteria:**

1. `ClipboardExportService.CopyToClipboardAsync()` includes try-catch for clipboard exceptions
2. Service catches `System.Runtime.InteropServices.COMException` (clipboard locked)
3. Service logs exception to Debug output
4. Service returns `false` when clipboard operation fails
5. ViewModel detects clipboard failure and sets StatusMessage:
   - "Unable to copy to clipboard. The clipboard may be in use by another application."
6. Error message suggests retry: "Please try again."
7. Unit tests validate clipboard failure handling (mock clipboard exceptions)
8. Manual testing: lock clipboard with another app, trigger copy, verify error message

## Story 8.6: Handle Empty Device List Gracefully

As a **user**,
I want **appropriate messaging when no USB devices are connected**,
so that **I know the application is working but no devices are detected** (per FR15).

**Acceptance Criteria:**

1. When `Devices` collection is empty, `IsEmpty = true` (implemented in Epic 6)
2. Empty state view displays message: "No USB devices connected"
3. Instructions provided: "Plug in a USB storage device and click Refresh"
4. Empty state includes large USB icon for visual clarity
5. Refresh button prominently displayed in empty state
6. No error styling (empty state is normal condition, not error)
7. Manual testing validates:
   - Launch app with no USB devices
   - Empty state appears with message and Refresh button
   - Plug in device, click Refresh
   - Device appears in list

## Story 8.7: Handle Internal SD Card Readers and Edge Case Devices

As a **user**,
I want **internal SD card readers and non-removable devices filtered out**,
so that **I only see actual USB storage devices I can whitelist** (per FR5).

**Acceptance Criteria:**

1. Device filtering logic (implemented in Epic 2) excludes:
   - Internal SD card readers (SystemDeviceID contains "SD" or "MMC")
   - Non-USB storage (SATA/PCIe drives)
   - Virtual drives (network drives, mounted ISOs)
2. Filtering logic tested with edge case devices:
   - USB-connected SD card reader (should be included)
   - Built-in laptop SD card slot (should be excluded)
   - USB hard drive enclosure (should be included)
3. Unit tests validate filtering logic with mock edge case devices
4. Manual testing with 5+ different device types validates filtering accuracy
5. Debug logging shows filtered device count (e.g., "Filtered out 2 internal devices")

## Story 8.8: Add Retry Mechanism for Device Enumeration

As a **user**,
I want **the ability to retry device enumeration if initial load fails**,
so that **I can recover from temporary errors without restarting the app**.

**Acceptance Criteria:**

1. Refresh button available even when device enumeration fails initially
2. StatusMessage displays error with suggestion: "Unable to load devices. Click Refresh to try again."
3. User can click Refresh to re-attempt enumeration
4. Retry logic identical to initial enumeration (no special retry handling needed)
5. Manual testing validates:
   - Simulate enumeration failure (disconnect USB controller or mock error)
   - Error message displayed with Refresh button
   - Click Refresh
   - Devices load successfully (if issue resolved)

## Story 8.9: Validate Error Message Clarity and Actionability

As a **QA engineer**,
I want **all error messages reviewed for clarity and actionability**,
so that **users understand what went wrong and what to do next**.

**Acceptance Criteria:**

1. All error messages documented in `docs/error-messages.md` with:
   - Error scenario
   - Error message text
   - User action guidance
2. Error messages follow guidelines:
   - No technical jargon or stack traces
   - Clear explanation of what happened
   - Actionable guidance on what user should do
   - Friendly, non-alarming tone
3. Error messages reviewed by stakeholders (product manager, UX, technical writer)
4. Sample error messages:
   - "Unable to access USB devices. Try running the application again."
   - "Serial Number unavailable - this device cannot be whitelisted by serial"
   - "Unable to copy to clipboard. The clipboard may be in use by another application. Please try again."
5. Error messages tested with users (or simulated user feedback)
6. Any confusing or unclear messages rewritten based on feedback

## Story 8.10: Test Edge Cases Comprehensively

As a **QA engineer**,
I want **comprehensive edge case testing performed**,
so that **application behaves predictably in unusual scenarios**.

**Acceptance Criteria:**

1. Edge case testing completed for scenarios:
   - No USB devices connected
   - USB device with no serial number
   - USB device with malformed Device Instance Path
   - USB device with missing VID/PID
   - Internal SD card reader (should be filtered)
   - USB-connected SD card reader (should be included)
   - Multiple USB devices (10+) connected simultaneously
   - USB device disconnected during enumeration
   - Clipboard locked by another application
   - Application launched without Windows.Devices.Enumeration API permissions (unlikely but possible)
2. Test results documented in `docs/edge-case-testing.md`:
   - Scenario description
   - Expected behavior
   - Actual behavior
   - Pass/fail status
3. Failed edge cases triaged as bugs and fixed (or documented as known limitations)
4. Test coverage validated: all known edge cases tested before release
