using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UsbDeviceInspector.Services.Interfaces;
using Windows.Devices.Enumeration;

namespace UsbDeviceInspector.Services;

/// <summary>
/// Service that enumerates USB storage devices using the Windows.Devices.Enumeration API.
/// </summary>
/// <remarks>
/// <para>
/// This service operates without requiring administrator privileges and works on Standard User accounts.
/// Filtering excludes internal SD card readers and non-USB storage devices per FR5.
/// </para>
/// <para>
/// All async methods use <c>ConfigureAwait(false)</c> to avoid capturing the synchronization context,
/// ensuring the service layer does not block the UI thread and preventing potential deadlocks
/// when called from UI code. This is intentional - service layer code should not depend on
/// returning to any specific synchronization context.
/// </para>
/// </remarks>
public class DeviceEnumerationService : IDeviceEnumerationService
{
    /// <summary>
    /// Prefixes that indicate internal SD/MMC card readers (should be excluded).
    /// </summary>
    private static readonly string[] SdMmcPrefixes = { "SD\\", "SDBUS\\", "MMC\\" };

    /// <summary>
    /// Backing field for the last refresh timestamp.
    /// </summary>
    private DateTimeOffset? _lastRefreshTime;

    /// <inheritdoc/>
    public DateTimeOffset? LastRefreshTime => _lastRefreshTime;

    /// <summary>
    /// Prefixes that indicate internal drives (should be excluded).
    /// </summary>
    private static readonly string[] InternalDrivePrefixes = { "SCSI\\", "SATA\\", "NVME\\", "PCIE\\", "IDE\\" };

    /// <summary>
    /// The USB prefix that indicates a USB-connected device.
    /// </summary>
    private const string UsbPrefix = "USB\\";

    /// <summary>
    /// Asynchronously enumerates USB storage devices connected to the system.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a collection of <see cref="DeviceInformation"/> objects for USB storage devices only.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method executes entirely on background thread pool threads and does not block the UI thread.
    /// The Windows.Devices.Enumeration API call uses <c>ConfigureAwait(false)</c> to avoid capturing
    /// the synchronization context, ensuring optimal performance and preventing potential deadlocks.
    /// </para>
    /// <para>
    /// Post-enumeration filtering is applied to exclude internal SD card readers and non-USB devices.
    /// Only devices with a <c>USB\</c> prefix in their Device Instance Path are included.
    /// </para>
    /// <para>
    /// Performance: Completes within 3 seconds for up to 10 connected devices (NFR1).
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<DeviceInformation>> EnumerateDevicesAsync()
    {
        Debug.WriteLine("DeviceEnumerationService: Starting USB storage device enumeration...");

        var devices = await DeviceInformation.FindAllAsync(DeviceClass.PortableStorageDevice)
            .AsTask()
            .ConfigureAwait(false);
        var originalCount = devices.Count;

        Debug.WriteLine($"DeviceEnumerationService: Enumeration complete. Found {originalCount} device(s) before filtering.");

        var filteredDevices = FilterUsbStorageDevices(devices);
        var filteredCount = filteredDevices.Count();

        Debug.WriteLine($"DeviceEnumerationService: Filtering complete. {filteredCount} USB storage device(s) after filtering (excluded {originalCount - filteredCount}).");

        _lastRefreshTime = DateTimeOffset.UtcNow;

        return filteredDevices;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DeviceInformation>> RefreshDevicesAsync()
    {
        Debug.WriteLine("DeviceEnumerationService: Starting device refresh...");

        var devices = await EnumerateDevicesAsync().ConfigureAwait(false);

        Debug.WriteLine($"DeviceEnumerationService: Refresh complete. Found {devices.Count()} device(s). LastRefreshTime: {_lastRefreshTime}");

        return devices;
    }

    /// <summary>
    /// Gets the Advanced Query Syntax (AQS) device selector for portable storage devices.
    /// </summary>
    /// <returns>An AQS filter string targeting portable storage devices.</returns>
    /// <remarks>
    /// This returns the default selector for PortableStorageDevice class.
    /// Post-enumeration filtering is applied separately to exclude SD/MMC and non-USB devices
    /// since AQS cannot fully express these complex filtering rules.
    /// </remarks>
    public static string GetDeviceSelector()
    {
        return DeviceInformation.CreateFromIdAsync(DeviceClass.PortableStorageDevice.ToString()).ToString()
            ?? DeviceClass.PortableStorageDevice.ToString();
    }

    /// <summary>
    /// Filters a collection of devices to include only USB-connected storage devices.
    /// </summary>
    /// <param name="devices">The collection of devices to filter.</param>
    /// <returns>A filtered collection containing only USB storage devices.</returns>
    /// <remarks>
    /// Applies both SD/MMC exclusion and USB connection verification.
    /// Devices must have a USB\ prefix and must not be internal SD card readers.
    /// </remarks>
    public static IEnumerable<DeviceInformation> FilterUsbStorageDevices(IEnumerable<DeviceInformation> devices)
    {
        if (devices == null)
        {
            return Enumerable.Empty<DeviceInformation>();
        }

        return devices.Where(device =>
        {
            var devicePath = device?.Id;
            if (string.IsNullOrEmpty(devicePath))
            {
                return false;
            }

            return IsUsbDevicePath(devicePath) && !IsInternalSdCardReaderPath(devicePath);
        });
    }

    /// <summary>
    /// Determines whether the specified device instance path indicates a USB-connected device.
    /// </summary>
    /// <param name="deviceInstancePath">The device instance path to check.</param>
    /// <returns>
    /// <c>true</c> if the path starts with "USB\" indicating a USB connection;
    /// <c>false</c> if the path indicates an internal connection (SATA, NVME, PCIE, IDE, SCSI) or is invalid.
    /// </returns>
    /// <remarks>
    /// USB device instance paths follow the pattern: USB\VID_xxxx&amp;PID_xxxx\serial
    /// Internal drives typically have prefixes like SCSI\, SATA\, NVME\, PCIE\, or IDE\.
    /// </remarks>
    public static bool IsUsbDevicePath(string? deviceInstancePath)
    {
        if (string.IsNullOrEmpty(deviceInstancePath))
        {
            return false;
        }

        // Check if it's an internal drive first (exclude these)
        foreach (var prefix in InternalDrivePrefixes)
        {
            if (deviceInstancePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        // Must start with USB\ to be considered a USB device
        return deviceInstancePath.StartsWith(UsbPrefix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified device instance path indicates an internal SD card reader.
    /// </summary>
    /// <param name="deviceInstancePath">The device instance path to check.</param>
    /// <returns>
    /// <c>true</c> if the path indicates an internal SD or MMC card reader;
    /// <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// Internal SD card readers typically have device instance paths starting with
    /// SD\, SDBUS\, or MMC\ prefixes. USB-connected card readers start with USB\
    /// and should be included (return false).
    /// </remarks>
    public static bool IsInternalSdCardReaderPath(string? deviceInstancePath)
    {
        if (string.IsNullOrEmpty(deviceInstancePath))
        {
            return false;
        }

        foreach (var prefix in SdMmcPrefixes)
        {
            if (deviceInstancePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
