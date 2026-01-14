# Security

**Overview:** This section defines MANDATORY security requirements for AI developers and human engineers. The USB Device Inspector has a minimal attack surface (offline desktop app, no network calls, no persistence), but device serial numbers may contain sensitive information in regulated industries.

## Input Validation

**Validation Library:** Built-in C# null-checking and string validation (no external validation framework required)

**Validation Location:** Service layer - all inputs to service methods MUST be validated before processing

**Required Rules:**
- **All Windows API outputs MUST be validated:** Device Instance Paths, DeviceInformation properties, HardwareIds arrays - never trust Windows API to return well-formed data
- **Validation at service boundary before processing:** Check for null, empty strings, invalid formats before regex parsing or string manipulation
- **Whitelist approach preferred over blacklist:** For Device Instance Path parsing, explicitly match expected `USB\VID_xxxx&PID_xxxx\Serial` pattern rather than blacklisting invalid characters
- **Fail safe:** If validation fails, throw descriptive exception (e.g., `DeviceParsingException`) rather than returning null or default values that might propagate through system

**Example Validation:**
```csharp
public (string VendorId, string ProductId)? ExtractVidPid(string deviceInstancePath)
{
    if (string.IsNullOrWhiteSpace(deviceInstancePath))
        throw new ArgumentException("Device Instance Path cannot be null or empty", nameof(deviceInstancePath));

    if (!deviceInstancePath.StartsWith("USB\\", StringComparison.OrdinalIgnoreCase))
        return null; // Not a USB device, return null (valid case for filtering)

    // Proceed with regex extraction...
}
```

## Authentication & Authorization

**Not Applicable** - Single-user offline desktop application with no user accounts, no authentication, no authorization checks. Windows OS handles application launch permissions (Standard User account sufficient).

## Secrets Management

**Development:** No secrets required for development - application uses only local Windows APIs with no API keys or credentials

**Production:** No secrets in production builds - zero external service dependencies means no API keys, database connection strings, or service credentials

**Code Requirements:**
- NEVER hardcode device serial numbers or VID/PID combinations in source code (could enable device allowlist bypass)
- No secrets in logs or error messages (device serials NOT logged per privacy requirement)
- Configuration files not used for MVP (future settings in Phase 2 would use Windows App SDK ApplicationData storage)

## API Security

**Not Applicable** - Application does not expose any APIs (no REST endpoints, no web server, no RPC interfaces). Entirely client-side desktop application.

## Data Protection

**Encryption at Rest:** Not applicable - no persistent data storage. All device information exists only in memory during application runtime and is discarded on exit.

**Encryption in Transit:** Not applicable - no network communication. Clipboard data is transmitted via Windows clipboard API (unencrypted, per Windows OS design).

**PII Handling:**
- **Device serial numbers MAY be considered PII** in regulated industries (healthcare, finance) if correlated with user identity
- **Data Minimization:** Application collects only VID, PID, Serial Number, Friendly Name - no user-identifiable information beyond device serials
- **Data Retention:** Zero retention - data exists only in application memory, cleared on exit or refresh
- **No telemetry:** Application does not phone home, send analytics, or report usage statistics (per NFR10)

**Logging Restrictions:**
- NEVER log full Device Instance Paths in production builds (contains serial numbers)
- NEVER log clipboard contents (may contain sensitive device information)
- Only log anonymized error types (e.g., "MissingSerialNumber") and device counts
- Development-only logging (Debug.WriteLine) acceptable for troubleshooting but stripped in Release builds

**Example Safe Logging:**
```csharp
// ❌ UNSAFE - logs device serial
_logger.LogWarning("Device parsing failed for {DeviceInstancePath}", deviceInstancePath);

// ✅ SAFE - logs anonymized information
_logger.LogWarning("Device parsing failed: {ErrorType}, DeviceId: {DeviceIdHash}",
    DeviceErrorType.MissingSerialNumber,
    ComputeHash(deviceInfo.Id));
```

## Dependency Security

**Scanning Tool:** GitHub Dependabot (enabled on repository)

**Update Policy:**
- **Critical/High severity vulnerabilities:** Immediate update within 48 hours of disclosure, emergency hotfix release if in production
- **Medium severity vulnerabilities:** Update within 2 weeks during next planned release cycle
- **Low severity vulnerabilities:** Update during major version increments (quarterly)
- **Transitive dependencies:** Monitor Dependabot alerts for indirect dependencies in NuGet package tree

**Approval Process for New Dependencies:**
1. Evaluate necessity - prefer built-in .NET libraries over third-party packages (avoid dependency bloat)
2. Check package authenticity - only Microsoft-published or well-established OSS packages (>1M downloads)
3. Review license compatibility - MIT/Apache 2.0 preferred, no GPL/AGPL (would require open-sourcing commercial forks)
4. Security audit - check NVD database for known CVEs in requested version
5. Dependency approval logged in PR review comments before merge

**Current Trusted Dependencies:**
- `Microsoft.WindowsAppSDK` - Official Microsoft package for WinUI3
- `CommunityToolkit.Mvvm` - Microsoft-maintained MVVM toolkit
- `xUnit`, `FluentAssertions`, `NSubstitute` - Industry-standard test frameworks with long track records

## Security Testing

**SAST Tool:** Roslyn Analyzers (built into .NET SDK 8.0) + SonarCloud (Phase 2)

**SAST Rules Enabled:**
- **CA2100:** Review SQL queries for security vulnerabilities (not applicable - no database)
- **CA2109:** Review visible event handlers (WinUI3 command handlers checked for input validation)
- **CA3001-CA3012:** Security rules for encryption, injection, XSS, CSRF (limited applicability to desktop app)
- **Custom Rule:** Regex DoS detection - any regex pattern without timeout is flagged (prevents ReDoS attacks on malformed Device Instance Paths)

**DAST Tool:** Not applicable for MVP - desktop applications cannot be dynamically scanned like web applications. Manual penetration testing deferred to Phase 2 if Microsoft Store publication is pursued.

**Penetration Testing:** Not planned for MVP. Phase 2 Microsoft Store publication would trigger:
- Manual security code review by Microsoft certification team
- Automated binary analysis for malware signatures
- Third-party penetration test if handling healthcare/finance data (HIPAA/PCI compliance assessment)

## Threat Model

**Primary Threats Mitigated:**
1. **Malicious USB Device:** Device with malformed Device Instance Path attempting code injection via regex
   - **Mitigation:** Regex with timeout, input validation, defensive parsing with try-catch
2. **Clipboard Hijacking:** Malware monitoring clipboard for device identifiers
   - **Mitigation:** None - inherent limitation of clipboard API. User education in documentation.
3. **DLL Injection:** Malicious software injecting code into application process
   - **Mitigation:** Code signing MSIX package, Windows Defender integration
4. **Dependency Vulnerability:** Compromised NuGet package in supply chain
   - **Mitigation:** Dependabot scanning, manual review of dependency updates, lock file verification

**Out of Scope:**
- Physical device tampering (e.g., USB Rubber Ducky) - not detectable by application software
- Windows OS vulnerabilities - rely on Windows Update for OS-level patches
- Social engineering attacks (user tricked into installing malware) - not preventable by application architecture