using System.Threading.Tasks;
using UsbDeviceInspector.Models;

namespace UsbDeviceInspector.Services.Interfaces;

/// <summary>
/// Defines the contract for parsing USB device properties from Device Instance Paths.
/// </summary>
/// <remarks>
/// <para>
/// This service is responsible for extracting VID (Vendor ID), PID (Product ID), and
/// Serial Number from the Device Instance Path of USB devices.
/// </para>
/// <para>
/// The service is stateless and operates by mutating the provided <see cref="UsbDevice"/>
/// object in-place for performance optimization during batch processing operations.
/// </para>
/// </remarks>
public interface IDeviceParsingService
{
    /// <summary>
    /// Parses device properties to extract VID, PID, and Serial Number from the device's
    /// Device Instance Path, updating the device object in-place.
    /// </summary>
    /// <param name="device">
    /// The USB device to parse. Properties (VendorId, ProductId, SerialNumber, IsValid)
    /// are modified in-place based on parsing results.
    /// </param>
    /// <returns>
    /// <c>true</c> if parsing succeeded and required properties (VID, PID) were extracted;
    /// <c>false</c> if the device is null or required properties could not be extracted.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method mutates the <paramref name="device"/> parameter rather than returning
    /// a new object. This design decision optimizes for batch processing scenarios where
    /// many devices are parsed in sequence.
    /// </para>
    /// <para>
    /// When parsing fails, the <see cref="UsbDevice.IsValid"/> property is set to <c>false</c>
    /// and detailed error information may be stored in <see cref="UsbDevice.ErrorMessage"/>.
    /// </para>
    /// </remarks>
    bool ParseDeviceProperties(UsbDevice device);

    /// <summary>
    /// Asynchronously parses device properties, querying Windows for parent USB device information
    /// when VID/PID is not directly available (e.g., WPD-enumerated devices).
    /// </summary>
    /// <param name="device">
    /// The USB device to parse. Properties (VendorId, ProductId, SerialNumber, IsValid)
    /// are modified in-place based on parsing results.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if parsing
    /// succeeded and required properties (VID, PID) were extracted; <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method first attempts synchronous parsing via <see cref="ParseDeviceProperties"/>.
    /// If that fails (e.g., for WPD devices), it queries Windows for the parent USB device.
    /// </para>
    /// </remarks>
    Task<bool> ParseDevicePropertiesAsync(UsbDevice device);
}
