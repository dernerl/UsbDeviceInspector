# External APIs

**Not Applicable** - The USB Device Inspector is an entirely offline desktop application with zero external API integrations. All functionality relies exclusively on:

- **Windows Runtime APIs** (Windows.Devices.Enumeration, Windows.ApplicationModel.DataTransfer) - These are local OS APIs, not external web services
- **Windows Clipboard** - Local system resource, not an external service

No network calls, no REST APIs, no third-party services. This design decision aligns with PRD requirements for offline-only operation (NFR10), zero telemetry (NFR10), and enterprise security compliance.
