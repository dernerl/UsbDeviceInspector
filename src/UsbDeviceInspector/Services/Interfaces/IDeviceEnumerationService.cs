using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace UsbDeviceInspector.Services.Interfaces;

/// <summary>
/// Provides functionality for enumerating USB storage devices connected to the system.
/// </summary>
public interface IDeviceEnumerationService
{
    /// <summary>
    /// Asynchronously enumerates all connected USB storage devices.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a collection of <see cref="DeviceInformation"/> objects representing the connected USB storage devices.
    /// </returns>
    /// <remarks>
    /// This method uses the Windows.Devices.Enumeration API and does not require administrator privileges.
    /// </remarks>
    Task<IEnumerable<DeviceInformation>> EnumerateDevicesAsync();
}