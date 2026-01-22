using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using UsbDeviceInspector.Models;

namespace UsbDeviceInspector.Services.Interfaces;

/// <summary>
/// Provides functionality for enumerating USB storage devices connected to the system.
/// </summary>
public interface IDeviceEnumerationService
{
    /// <summary>
    /// Gets the timestamp of the last successful device enumeration or refresh operation.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the UTC time of the last successful enumeration,
    /// or <c>null</c> if no enumeration has been performed yet.
    /// </value>
    DateTimeOffset? LastRefreshTime { get; }

    /// <summary>
    /// Asynchronously enumerates all connected USB storage devices.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a collection of <see cref="UsbDevice"/> objects representing the connected USB storage devices.
    /// Each <see cref="UsbDevice"/> object is mapped from <see cref="DeviceInformation"/> with additional
    /// metadata (System.ItemNameDisplay, System.Devices.Manufacturer, System.Devices.HardwareIds,
    /// System.Devices.DeviceInstanceId). Returns an empty collection (not null) if no devices are found.
    /// </returns>
    /// <remarks>
    /// This method uses the Windows.Devices.Enumeration API and does not require administrator privileges.
    /// Device mapping failures are logged but do not prevent successful devices from being returned.
    /// </remarks>
    Task<IEnumerable<UsbDevice>> EnumerateDevicesAsync();

    /// <summary>
    /// Asynchronously refreshes the list of connected USB storage devices.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the updated collection of <see cref="UsbDevice"/> objects. Returns an empty collection (not null)
    /// if no devices are found.
    /// </returns>
    /// <remarks>
    /// This method re-enumerates all devices and updates <see cref="LastRefreshTime"/>.
    /// Any previously cached device information is implicitly replaced by the new enumeration results.
    /// Device mapping failures are logged but do not prevent successful devices from being returned.
    /// </remarks>
    Task<IEnumerable<UsbDevice>> RefreshDevicesAsync();
}