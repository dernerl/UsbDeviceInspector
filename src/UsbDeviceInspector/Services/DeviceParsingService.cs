using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UsbDeviceInspector.Models;
using UsbDeviceInspector.Services.Interfaces;
using Windows.Devices.Enumeration;

namespace UsbDeviceInspector.Services;

/// <summary>
/// Parses USB device properties to extract VID, PID, and Serial Number from Device Instance Paths.
/// </summary>
/// <remarks>
/// <para>
/// This service is stateless and contains pure parsing logic. It has no dependencies on other
/// services and can be safely registered as a singleton in the DI container.
/// </para>
/// <para>
/// The parsing logic extracts device identifiers from the Device Instance Path format:
/// <c>USB\VID_XXXX&amp;PID_XXXX\SerialNumber</c>
/// </para>
/// <para>
/// Actual regex-based parsing logic will be implemented in Stories 3.2-3.4.
/// This story establishes the service structure with stub implementations.
/// </para>
/// </remarks>
public class DeviceParsingService : IDeviceParsingService
{
    /// <summary>
    /// Compiled regex pattern for extracting Vendor ID (VID) from Device Instance Path.
    /// Matches pattern "VID_" followed by exactly 4 hexadecimal characters (case-insensitive).
    /// </summary>
    /// <example>
    /// Input: "USB\VID_0781&amp;PID_5581\4C530001231120115142"
    /// Match: "VID_0781" with capture group "0781"
    /// </example>
    private static readonly Regex VidRegex = new(
        @"VID_([0-9A-Fa-f]{4})",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    /// <summary>
    /// Compiled regex pattern for extracting Product ID (PID) from Device Instance Path.
    /// Matches pattern "PID_" followed by exactly 4 hexadecimal characters (case-insensitive).
    /// </summary>
    /// <example>
    /// Input: "USB\VID_0781&amp;PID_5581\4C530001231120115142"
    /// Match: "PID_5581" with capture group "5581"
    /// </example>
    private static readonly Regex PidRegex = new(
        @"PID_([0-9A-Fa-f]{4})",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    /// <summary>
    /// Compiled regex pattern for extracting USBSTOR device instance ID from WPD paths.
    /// Matches the USBSTOR portion embedded in WPD device paths.
    /// </summary>
    /// <example>
    /// Input: "SWD\WPDBUSENUM\_??_USBSTOR#Disk&amp;Ven_Generic&amp;Prod_Flash_Disk&amp;Rev_8.07#512067C4&amp;0#{53f56307-b6bf-11d0-94f2-00a0c91efb8b}"
    /// Match: "USBSTOR#Disk&amp;Ven_Generic&amp;Prod_Flash_Disk&amp;Rev_8.07#512067C4&amp;0"
    /// </example>
    private static readonly Regex UsbStorRegex = new(
        @"USBSTOR#([^#]+)#([^#]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceParsingService"/> class.
    /// </summary>
    /// <remarks>
    /// This service has no dependencies and is stateless. The parameterless constructor
    /// enables simple instantiation and singleton registration in the DI container.
    /// </remarks>
    public DeviceParsingService()
    {
        // No dependencies - stateless service
    }

    /// <inheritdoc />
    /// <remarks>
    /// <para>
    /// Current implementation is a stub that returns <c>false</c> for all devices.
    /// Actual parsing logic will be implemented in subsequent stories:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Story 3.2: VID/PID extraction from Device Instance Path</description></item>
    /// <item><description>Story 3.3: Serial Number extraction</description></item>
    /// <item><description>Story 3.4: Regex pattern implementation with compiled patterns</description></item>
    /// <item><description>Story 3.5: Error handling and validation</description></item>
    /// </list>
    /// </remarks>
    public bool ParseDeviceProperties(UsbDevice device)
    {
        // Null check - return false without throwing
        if (device is null)
        {
            return false;
        }

        // Try to extract VID/PID from multiple sources:
        // 1. First try DeviceInstancePath (direct USB enumeration format)
        // 2. If not found, search through HardwareIds array
        // 3. If not found, try ParentDevicePath (may contain USB device path)
        var vidPidResult = ExtractVidPid(device.DeviceInstancePath);

        if (vidPidResult is null && device.HardwareIds.Length > 0)
        {
            // Search HardwareIds array for VID/PID
            foreach (var hardwareId in device.HardwareIds)
            {
                vidPidResult = ExtractVidPid(hardwareId);
                if (vidPidResult is not null)
                {
                    break;
                }
            }
        }

        if (vidPidResult is null && !string.IsNullOrEmpty(device.ParentDevicePath))
        {
            // Try parent device path (for WPD-enumerated devices, parent may be USB device)
            vidPidResult = ExtractVidPid(device.ParentDevicePath);
        }

        if (vidPidResult is null)
        {
            // Extraction failed - leave VendorId/ProductId empty, mark invalid
            device.IsValid = false;
            return false;
        }

        // Set extracted values
        device.VendorId = vidPidResult.Value.vid;
        device.ProductId = vidPidResult.Value.pid;

        // Return true - VID/PID extraction successful (serial number parsing in Story 3.3)
        return true;
    }

    /// <summary>
    /// Extracts Vendor ID (VID) and Product ID (PID) from a Device Instance Path string.
    /// </summary>
    /// <param name="input">The Device Instance Path string to parse (e.g., "USB\VID_0781&amp;PID_5581\4C530001231120115142").</param>
    /// <returns>
    /// A tuple containing the extracted VID and PID in uppercase hexadecimal format (4 characters each),
    /// or <c>null</c> if either VID or PID could not be extracted.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method uses compiled regex patterns for performance and supports case-insensitive matching
    /// (VID_, vid_, Vid_ are all valid). Output values are always returned in uppercase.
    /// </para>
    /// <para>
    /// The method returns <c>null</c> in the following cases:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Input string is null or empty</description></item>
    /// <item><description>VID pattern not found in input</description></item>
    /// <item><description>PID pattern not found in input</description></item>
    /// <item><description>VID or PID contains non-hexadecimal characters</description></item>
    /// </list>
    /// </remarks>
    internal static (string vid, string pid)? ExtractVidPid(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return null;
        }

        var vidMatch = VidRegex.Match(input);
        var pidMatch = PidRegex.Match(input);

        if (!vidMatch.Success || !pidMatch.Success)
        {
            return null;
        }

        // Return extracted values in uppercase (4-character hex format)
        return (
            vid: vidMatch.Groups[1].Value.ToUpperInvariant(),
            pid: pidMatch.Groups[1].Value.ToUpperInvariant()
        );
    }

    /// <inheritdoc />
    public async Task<bool> ParseDevicePropertiesAsync(UsbDevice device)
    {
        // First try synchronous parsing
        if (ParseDeviceProperties(device))
        {
            return true;
        }

        // Synchronous parsing failed - try to resolve parent USB device for WPD devices
        if (device is null || string.IsNullOrEmpty(device.DeviceInstancePath))
        {
            return false;
        }

        // Check if this is a WPD-enumerated device with USBSTOR reference
        var usbStorMatch = UsbStorRegex.Match(device.DeviceInstancePath);
        if (!usbStorMatch.Success)
        {
            Debug.WriteLine($"DeviceParsingService: No USBSTOR reference found in path: {device.DeviceInstancePath}");
            return false;
        }

        // Reconstruct USBSTOR device instance path
        var usbStorDeviceId = $"USBSTOR\\{usbStorMatch.Groups[1].Value}\\{usbStorMatch.Groups[2].Value}";
        Debug.WriteLine($"DeviceParsingService: Found USBSTOR reference: {usbStorDeviceId}");

        try
        {
            // Query Windows for the USBSTOR device to get its parent
            var usbStorDevice = await DeviceInformation.CreateFromIdAsync(
                usbStorDeviceId,
                new[] { "System.Devices.Parent" },
                DeviceInformationKind.Device
            ).AsTask().ConfigureAwait(false);

            if (usbStorDevice?.Properties != null &&
                usbStorDevice.Properties.TryGetValue("System.Devices.Parent", out var parentValue) &&
                parentValue is string parentPath)
            {
                Debug.WriteLine($"DeviceParsingService: USBSTOR parent device: {parentPath}");
                var vidPidResult = ExtractVidPid(parentPath);
                if (vidPidResult is not null)
                {
                    device.VendorId = vidPidResult.Value.vid;
                    device.ProductId = vidPidResult.Value.pid;
                    device.IsValid = true;
                    Debug.WriteLine($"DeviceParsingService: Extracted VID={device.VendorId}, PID={device.ProductId} from parent");
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"DeviceParsingService: Failed to query USBSTOR device: {ex.Message}");
        }

        // Extraction failed
        device.IsValid = false;
        return false;
    }
}
