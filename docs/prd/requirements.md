# Requirements

## Functional

**FR1:** The system shall enumerate all currently connected USB storage devices using Windows.Devices.Enumeration API without requiring administrator privileges.

**FR2:** The system shall extract and display the following device properties for each USB storage device: Friendly Name, Manufacturer, Vendor ID (VID), Product ID (PID), and Serial Number.

**FR3:** The system shall parse Vendor ID and Product ID from the device's HardwareIds property array in hexadecimal format (e.g., VID_0781, PID_5581).

**FR4:** The system shall extract Serial Number from the Device Instance Path following the pattern `USB\VID_xxxx&PID_xxxx\SerialNumber`.

**FR5:** The system shall filter device enumeration results to exclude internal SD card readers and non-USB storage devices.

**FR6:** The system shall generate CrowdStrike Falcon Device Control Combined ID format: `VID_xxxx&PID_xxxx\SerialNumber` for each detected device.

**FR7:** The system shall generate CrowdStrike Falcon Device Control Manual Entry format containing separate fields for VID, PID, and Serial Number.

**FR8:** The system shall provide a "Copy for CrowdStrike" action that copies both Combined ID and Manual Entry formats to the Windows clipboard for IT administrator use.

**FR9:** The system shall provide a "Copy for Helpdesk" action that copies device information in human-readable format suitable for inclusion in support tickets by end users.

**FR10:** The system shall provide visual confirmation feedback when data is successfully copied to the clipboard (button state change or notification).

**FR11:** The system shall provide a manual "Refresh" button to re-enumerate devices and detect newly connected USB devices.

**FR12:** The system shall automatically enumerate devices on application launch.

**FR13:** The system shall display device information in a card-based layout with one card per detected USB device.

**FR14:** The system shall display clear error messages when devices lack serial numbers or when Device Instance Path parsing fails.

**FR15:** The system shall gracefully handle the scenario where no USB storage devices are connected with appropriate user-facing messaging.

## Non Functional

**NFR1:** The system shall complete device enumeration within 3 seconds for workstations with up to 10 connected USB devices.

**NFR2:** The system shall maintain UI responsiveness during device enumeration by executing enumeration operations asynchronously on background threads.

**NFR3:** The system shall operate successfully on Standard-User (non-administrator) Windows accounts without UAC prompts or elevation requirements.

**NFR4:** The system shall target Windows 10 version 1809 or later and Windows 11 as supported platforms.

**NFR5:** The system shall maintain a memory footprint under 100MB during typical operation.

**NFR6:** The system shall achieve a cold start time under 2 seconds on modern hardware (SSD, 8GB+ RAM).

**NFR7:** The system shall use WinUI 3 and Fluent Design System to provide a native Windows 10/11 look and feel.

**NFR8:** The system shall require zero configuration or settings for basic operation (plug-and-play user experience).

**NFR9:** The exported CrowdStrike Combined ID format shall be validated to match CrowdStrike Falcon Device Control specification with 100% accuracy.

**NFR10:** The system shall not transmit any device data over the network or store any telemetry data externally (fully offline operation).

**NFR11:** The system shall not persist any device data to disk beyond standard Windows clipboard operations (no local database or file storage in MVP).

**NFR12:** The system shall successfully parse Device Instance Paths for at least 90% of USB storage devices from major manufacturers (SanDisk, Kingston, Samsung, etc.).
