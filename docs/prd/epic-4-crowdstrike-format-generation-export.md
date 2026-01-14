# Epic 4: CrowdStrike Format Generation & Export

**Epic Goal:** Implement CrowdStrike Falcon Device Control-compatible output format generation and clipboard export functionality, enabling IT administrators to copy device identifiers directly from the application and paste them into the CrowdStrike Falcon console without manual formatting. This epic delivers the core value proposition for IT administrators by reducing whitelisting time from 5-10 minutes to under 60 seconds through automated formatting and one-click clipboard export.

## Story 4.1: Create FormatService for Output Generation

As a **developer**,
I want **a dedicated service for generating export formats**,
so that **formatting logic is separated and can be extended for multiple output types**.

**Acceptance Criteria:**

1. `FormatService` class created in `Services/` folder
2. Service is stateless (no instance fields, pure formatting logic)
3. Service includes methods:
   - `string GenerateCrowdStrikeFormat(UsbDevice device)` for Combined ID + Manual Entry format
   - Method signatures include XML documentation
4. Service constructor accepts no dependencies
5. Unit tests validate service instantiation and method signatures
6. Service returns empty string or null when provided invalid UsbDevice (IsValid = false)
7. Debug assertions validate input parameters (device not null, required properties populated)

## Story 4.2: Generate CrowdStrike Combined ID Format

As an **IT administrator**,
I want **the Combined ID format generated as `VID_xxxx&PID_xxxx\SerialNumber`**,
so that **I can paste it directly into CrowdStrike Falcon Device Control** (per FR6).

**Acceptance Criteria:**

1. FormatService generates string in exact format: `VID_{VendorId}&PID_{ProductId}\{SerialNumber}`
2. VendorId and ProductId formatted as uppercase 4-digit hexadecimal (e.g., `0781`, `5581`)
3. SerialNumber included exactly as parsed (no transformations)
4. Format matches CrowdStrike specification with 100% accuracy (NFR9)
5. Unit tests validate format generation with:
   - Standard device (VID=0781, PID=5581, Serial=4C530001231124103024)
   - Device with lowercase hex values (should be uppercased)
   - Device with 3-digit hex (should be zero-padded to 4 digits)
6. Generated string includes no leading/trailing whitespace
7. Example output: `VID_0781&PID_5581\4C530001231124103024`
8. Manual validation: paste generated format into CrowdStrike Falcon Device Control test environment (or documented validation plan)

## Story 4.3: Generate CrowdStrike Manual Entry Format

As an **IT administrator**,
I want **the Manual Entry format with separate VID, PID, and Serial fields**,
so that **I can manually enter values into CrowdStrike form fields if needed** (per FR7).

**Acceptance Criteria:**

1. FormatService generates multi-line string with format:
   ```
   Vendor ID: 0781
   Product ID: 5581
   Serial Number: 4C530001231124103024
   ```
2. Each field on separate line with label and value separated by colon and space
3. VendorId and ProductId formatted as uppercase 4-digit hexadecimal
4. SerialNumber included exactly as parsed
5. Unit tests validate manual entry format generation with multiple device examples
6. Generated string uses `\r\n` (Windows line endings) for clipboard compatibility
7. Format includes no extra blank lines or trailing whitespace

## Story 4.4: Combine Both Formats into Single Clipboard Output

As an **IT administrator**,
I want **both Combined ID and Manual Entry formats in a single clipboard export**,
so that **I have both formats available depending on CrowdStrike UI context** (per FR8).

**Acceptance Criteria:**

1. `GenerateCrowdStrikeFormat()` returns single string containing both formats separated by blank line:
   ```
   VID_0781&PID_5581\4C530001231124103024

   Vendor ID: 0781
   Product ID: 5581
   Serial Number: 4C530001231124103024
   ```
2. Combined ID format appears first, followed by blank line, then Manual Entry format
3. Output uses Windows line endings (`\r\n`) throughout
4. Unit tests validate complete format structure
5. Manual testing: copy format and paste into Notepad to verify correct multi-line formatting
6. Format includes section headers for clarity (optional enhancement)

## Story 4.5: Create ClipboardExportService

As a **developer**,
I want **a service that handles Windows clipboard operations**,
so that **clipboard logic is separated and can be tested independently**.

**Acceptance Criteria:**

1. `ClipboardExportService` class created in `Services/` folder
2. Service includes method `Task<bool> CopyToClipboardAsync(string content)`
3. Method uses `Windows.ApplicationModel.DataTransfer.DataPackage` API
4. Method sets clipboard content as plain text (not HTML or Rich Text)
5. Method returns `true` if clipboard operation succeeds, `false` if it fails
6. Service includes error handling for clipboard access failures (e.g., clipboard locked by another app)
7. Unit tests validate service instantiation and method signatures
8. Integration tests validate clipboard operations set system clipboard correctly

## Story 4.6: Implement CopyToClipboard Method with DataPackage

As a **developer**,
I want **clipboard export to use Windows.ApplicationModel.DataTransfer.DataPackage**,
so that **clipboard operations work reliably across Windows 10/11** (per Technical Assumptions).

**Acceptance Criteria:**

1. `CopyToClipboardAsync()` creates new `DataPackage` instance
2. Method calls `dataPackage.SetText(content)` to set plain text content
3. Method calls `Clipboard.SetContent(dataPackage)` to update system clipboard
4. Method executes on UI thread (clipboard operations require UI thread context)
5. Method includes try-catch block for clipboard exceptions
6. Method logs errors to Debug output when clipboard operation fails
7. Unit tests mock DataPackage operations (if possible) or integration tests validate real clipboard
8. Manual testing validates:
   - Copy text to clipboard
   - Paste into Notepad
   - Correct multi-line format appears

## Story 4.7: Add CopyForCrowdStrike Method to Service Layer

As a **developer**,
I want **a high-level service method that combines formatting and clipboard export**,
so that **ViewModels can trigger CrowdStrike export with a single call**.

**Acceptance Criteria:**

1. New method `Task<bool> ExportCrowdStrikeFormatAsync(UsbDevice device)` added to appropriate service (or new ExportService)
2. Method calls `FormatService.GenerateCrowdStrikeFormat(device)` to generate output
3. Method calls `ClipboardExportService.CopyToClipboardAsync(formattedOutput)` to copy to clipboard
4. Method returns `true` if export succeeds, `false` if formatting or clipboard operation fails
5. Method validates device is valid (`IsValid = true`) before attempting export
6. Method returns `false` immediately if device is invalid (no clipboard operation attempted)
7. Unit tests validate:
   - Successful export with valid device
   - Failed export with invalid device (IsValid = false)
   - Failed export when clipboard operation fails
8. Service includes debug logging for export operations (success/failure)

## Story 4.8: Validate CrowdStrike Format Accuracy

As a **QA engineer**,
I want **CrowdStrike format validated against official specification**,
so that **generated formats work in CrowdStrike Falcon Device Control with 100% accuracy** (per NFR9).

**Acceptance Criteria:**

1. CrowdStrike Falcon Device Control specification documented or referenced in code comments
2. Generated Combined ID format validated against specification (manual review or automated)
3. Test cases created with known-good device identifiers from CrowdStrike documentation
4. Unit tests validate format output matches expected output character-for-character
5. Manual testing: paste generated format into CrowdStrike Falcon Device Control test environment
6. Manual testing: verify device whitelisting succeeds using generated format
7. Test results documented in `docs/crowdstrike-format-validation.md`
8. Any format deviations or edge cases documented and addressed
