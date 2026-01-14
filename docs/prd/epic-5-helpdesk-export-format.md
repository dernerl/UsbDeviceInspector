# Epic 5: Helpdesk Export Format

**Epic Goal:** Create a human-readable device information export format optimized for end users to include in helpdesk support tickets when requesting USB device whitelisting. This format enables non-technical employees to provide complete device information to IT support staff without understanding technical concepts like VID/PID, reducing ticket incompleteness and accelerating the USB whitelisting approval workflow from days to hours.

## Story 5.1: Design Helpdesk Format Specification

As a **product manager**,
I want **a human-readable format specification defined**,
so that **developers know exactly what information to include and how to structure it** (per FR9).

**Acceptance Criteria:**

1. Helpdesk format specification documented in code comments or `docs/helpdesk-format-spec.md`
2. Format includes fields:
   - Device Name (friendly name)
   - Manufacturer
   - Model Information (derived from VID/PID or friendly name)
   - Serial Number
   - Technical Identifiers (VID/PID for IT staff reference)
3. Format uses clear labels understandable to non-technical users (no jargon)
4. Format example documented:
   ```
   USB Device Information for IT Support
   ======================================
   Device Name: SanDisk Ultra USB 3.0
   Manufacturer: SanDisk
   Serial Number: 4C530001231124103024

   Technical Details (for IT staff):
   Vendor ID: 0781
   Product ID: 5581
   ```
5. Format specification reviewed and approved by stakeholders (IT admins and helpdesk staff)
6. Specification includes guidelines for line length and readability

## Story 5.2: Implement GenerateHelpdeskFormat Method

As a **developer**,
I want **FormatService to generate helpdesk format from UsbDevice**,
so that **end users can copy device information in readable format** (per FR9).

**Acceptance Criteria:**

1. Method `string GenerateHelpdeskFormat(UsbDevice device)` added to FormatService
2. Method generates multi-line string with format:
   ```
   USB Device Information for IT Support
   ======================================
   Device Name: {FriendlyName}
   Manufacturer: {Manufacturer}
   Serial Number: {SerialNumber}

   Technical Details (for IT staff):
   Vendor ID: {VendorId}
   Product ID: {ProductId}
   ```
3. Method handles missing Manufacturer gracefully (shows "Unknown" if null/empty)
4. Method uses Windows line endings (`\r\n`)
5. Method includes header with visual separator (equals signs) for readability
6. Unit tests validate format generation with:
   - Standard device with all properties
   - Device with missing manufacturer
   - Device with long friendly name (verify no truncation)
7. Generated string is 10-15 lines total (scannable in ticket systems)

## Story 5.3: Add Copy for Helpdesk Functionality

As a **developer**,
I want **a service method to export helpdesk format to clipboard**,
so that **end users can trigger helpdesk export with a single action**.

**Acceptance Criteria:**

1. Method `Task<bool> ExportHelpdeskFormatAsync(UsbDevice device)` added to export service
2. Method calls `FormatService.GenerateHelpdeskFormat(device)` to generate output
3. Method calls `ClipboardExportService.CopyToClipboardAsync(formattedOutput)` to copy to clipboard
4. Method validates device is valid before export (IsValid = true)
5. Method returns `true` on successful export, `false` on failure
6. Unit tests validate successful export and failure scenarios
7. Method includes debug logging for helpdesk export operations

## Story 5.4: Add User Instructions to Helpdesk Format

As an **end user**,
I want **instructions included in the clipboard export**,
so that **I know how to use this information in my support ticket**.

**Acceptance Criteria:**

1. Helpdesk format includes footer with instructions:
   ```
   ======================================
   Copy this entire message into your support ticket when requesting USB device access.
   ```
2. Instructions are concise (1-2 sentences)
3. Instructions use friendly, non-technical language
4. Unit tests validate footer appears in generated format
5. Manual testing: copy helpdesk format and paste into mock email/ticket to validate readability

## Story 5.5: Handle Invalid Devices in Helpdesk Export

As an **end user**,
I want **helpdesk export to work even if device parsing partially failed**,
so that **I can still provide available information to IT support**.

**Acceptance Criteria:**

1. Helpdesk format includes all available information even if IsValid = false
2. Missing fields show placeholder text:
   - Serial Number: "Not available (contact IT for assistance)"
   - Vendor ID/Product ID: "Unable to determine" if parsing failed
3. Format includes note when information is incomplete:
   ```
   Note: This device information is incomplete. IT support may need to inspect the device in person.
   ```
4. Unit tests validate partial information export for devices with:
   - Missing serial number
   - Missing VID/PID
   - Missing manufacturer
5. Export still succeeds (returns true) even with incomplete device data
6. Manual testing validates end user can copy incomplete device info and paste into ticket

## Story 5.6: Validate Helpdesk Format Readability

As a **helpdesk staff member**,
I want **the helpdesk format to be clear and complete**,
so that **I can quickly identify the device and process whitelisting requests**.

**Acceptance Criteria:**

1. Manual testing with helpdesk staff (2+ participants) to validate format usability
2. Feedback collected on:
   - Information completeness (all necessary details included?)
   - Readability (easy to scan and understand?)
   - Compatibility with ticketing systems (Jira, ServiceNow, email)
3. Format adjustments made based on feedback
4. Final format approved by helpdesk representatives
5. Test results documented in `docs/helpdesk-format-validation.md`
6. Format tested by pasting into actual ticketing system (or mock system)

## Story 5.7: Document Helpdesk Workflow for End Users

As an **end user**,
I want **documentation explaining how to request USB device whitelisting**,
so that **I know when and how to use the helpdesk export feature**.

**Acceptance Criteria:**

1. User documentation created in `docs/end-user-guide.md` (or README section)
2. Documentation includes:
   - When to use "Copy for Helpdesk" (requesting USB device access)
   - Step-by-step instructions:
     1. Plug in USB device
     2. Launch USB Device Inspector
     3. Find your device in the list
     4. Click "Copy for Helpdesk"
     5. Paste into support ticket or email
   - Screenshot or mockup showing the UI (placeholder until Epic 6)
   - Example support ticket with pasted device information
3. Documentation uses simple, non-technical language
4. Documentation reviewed by non-technical stakeholder (or simulated user)
5. Documentation accessible from README.md
