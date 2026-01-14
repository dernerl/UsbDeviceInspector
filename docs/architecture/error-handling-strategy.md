# Error Handling Strategy

## General Approach

**Error Model:** Exception-based error handling with custom exception hierarchy for business logic errors. Windows API failures wrapped with descriptive context messages.

**Exception Hierarchy:**
- `UsbInspectorException` (base class) - All application-specific exceptions inherit from this
  - `DeviceEnumerationException` - Thrown when Windows API enumeration fails
  - `DeviceParsingException` - Thrown when device property extraction fails
  - `ClipboardAccessException` - Thrown when clipboard operations fail
  - `InvalidDeviceDataException` - Thrown when device data doesn't meet expected format

**Error Propagation:**
- Service layer methods throw specific exceptions with detailed error context
- ViewModel layer catches service exceptions, logs them, and updates UI-bindable error properties
- UI layer displays error messages inline (device cards) or via notifications (clipboard failures)
- Never propagate raw Windows Runtime exceptions to UI - always wrap with context

## Logging Standards

**Library:** `Microsoft.Extensions.Logging` 8.0.0 (abstraction layer) with `Debug` output provider for development

**Format:** Structured logging with JSON serialization for future file-based logging (Phase 2)

**Log Levels Definition:**
- **Trace:** Detailed device parsing steps (VID/PID extraction regex matches) - development only
- **Debug:** Service method entry/exit, enumeration counts, timing metrics - development only
- **Information:** Application lifecycle events (startup, shutdown), successful operations (devices enumerated, clipboard copy success)
- **Warning:** Recoverable errors (device parsing failures, devices without serial numbers), retry attempts before success
- **Error:** Unrecoverable operation failures (clipboard locked, Windows API access denied), exceptions caught by ViewModels
- **Critical:** Application-level failures preventing startup (DI container configuration failure, missing Windows API support)

**Required Context:**
- **Correlation ID:** Not required for MVP (no distributed tracing). Each enumeration operation gets unique `OperationId` (GUID) for log correlation within single session.
- **Service Context:** Service class name, method name, operation type (e.g., `DeviceEnumerationService.EnumerateUsbStorageDevicesAsync`)
- **User Context:** No user identification logged (privacy requirement). Device count and error counts only.

**Example Log Entry:**
```json
{
  "Timestamp": "2026-01-14T10:23:45.123Z",
  "Level": "Warning",
  "Message": "Device parsing failed for device",
  "OperationId": "a7f3c2d1-4e5f-6a7b-8c9d-0e1f2a3b4c5d",
  "Service": "DeviceParsingService",
  "Method": "TryParseDevice",
  "DeviceId": "USB\\VID_0781&PID_5581\\...",
  "ErrorType": "MissingSerialNumber",
  "Exception": null
}
```

## Error Handling Patterns

### External API Errors (Windows Runtime APIs)

**Retry Policy:**
- Device enumeration failures: 3 retry attempts with exponential backoff (500ms, 1000ms, 2000ms)
- Clipboard operations: Single retry after 1-second delay (clipboard often locked by other apps briefly)
- No retries for parsing errors (deterministic failures)

**Circuit Breaker:** Not implemented for MVP (single-user desktop app, no cascading failure risk). If Phase 2 adds WMI fallback, circuit breaker would prevent repeated WMI elevation prompts after 3 consecutive failures.

**Timeout Configuration:**
- Device enumeration: 10-second timeout per attempt (Windows API should respond in <3 seconds per NFR1, but allow buffer for slow systems)
- Clipboard operations: 5-second timeout (clipboard can be slow on enterprise systems with DLP software monitoring)

**Error Translation:**
- Windows Runtime `UnauthorizedAccessException` → `DeviceEnumerationException` with message "Windows denied access to device information. Ensure application is not blocked by security software."
- `COMException` from clipboard API → `ClipboardAccessException` with message "Clipboard is currently locked by another application. Please try again."
- Windows API timeout → `DeviceEnumerationException` with message "Device enumeration timed out. System may be under heavy load."

### Business Logic Errors

**Custom Exceptions:**
- `InvalidDeviceDataException` - Thrown when device lacks required properties (VID/PID missing entirely)
- `DeviceParsingException` - Thrown when Device Instance Path doesn't match expected USB pattern
- `UnsupportedDeviceException` - Thrown when device is internal SD card reader or non-USB device

**User-Facing Errors:**
- Displayed inline on device cards with warning icon (⚠️) for device-specific errors
- Toast notifications for system-level errors (clipboard failures, enumeration failures)
- Error messages written in plain language, avoid technical jargon:
  - ❌ "Regex match failed for Device Instance Path pattern"
  - ✅ "This device does not provide a serial number and cannot be used with CrowdStrike"

**Error Codes:** Not implemented for MVP (error types handled by `DeviceErrorType` enum). If Phase 2 adds telemetry, error codes would be added for aggregation (e.g., `ERR_DEV_001` for missing serial).

### Data Consistency

**Transaction Strategy:** Not applicable - application has no persistent state or database transactions. All data is transient in-memory models.

**Compensation Logic:** Not applicable - no distributed operations requiring compensation.

**Idempotency:**
- All operations are naturally idempotent:
  - Device enumeration can be called multiple times without side effects (read-only Windows API query)
  - Clipboard copy overwrites previous clipboard content (last-write-wins semantics)
  - Refresh operation clears and rebuilds device collection (full replacement, not incremental update)

**Partial Failure Handling:**
- Device parsing failures do not abort entire enumeration - successfully parsed devices still displayed
- `DeviceEnumerationResult` contains both `Devices` list and `Errors` list
- UI shows error cards inline with successful device cards, allowing users to use working devices while aware of problematic ones
