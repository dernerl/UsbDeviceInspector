using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Devices.Enumeration;

namespace UsbDeviceInspector.Models;

/// <summary>
/// Represents a USB storage device with all relevant properties for device identification.
/// This model serves as a data transfer object between the service layer and presentation layer,
/// providing strongly-typed access to device metadata.
/// </summary>
/// <remarks>
/// <para>
/// This class inherits from <see cref="ObservableObject"/> to provide automatic property change
/// notifications for UI binding. The <c>[ObservableProperty]</c> source generators are used to
/// reduce boilerplate code while maintaining full MVVM compatibility.
/// </para>
/// <para>
/// Properties marked as placeholders (VendorId, ProductId, SerialNumber, IsValid, ErrorMessage)
/// will be populated in subsequent epics during device parsing operations.
/// </para>
/// </remarks>
public partial class UsbDevice : ObservableObject
{
    /// <summary>
    /// Gets or sets the unique device identifier from Windows API.
    /// This is typically a long device path string (e.g., "\\?\USB#VID_0781&amp;PID_5581#...").
    /// </summary>
    [ObservableProperty]
    private string _id = string.Empty;

    /// <summary>
    /// Gets or sets the user-friendly device name (e.g., "SanDisk Ultra USB 3.0").
    /// Retrieved from the System.ItemNameDisplay property.
    /// </summary>
    [ObservableProperty]
    private string _friendlyName = string.Empty;

    /// <summary>
    /// Gets or sets the device manufacturer name. May be null if not exposed by device.
    /// Retrieved from the System.Devices.Manufacturer property.
    /// </summary>
    [ObservableProperty]
    private string? _manufacturer;

    /// <summary>
    /// Gets or sets the device vendor identifier as a 4-character hexadecimal string (e.g., "0781").
    /// Placeholder property - will be populated in Epic 3 during Device Instance Path parsing.
    /// </summary>
    [ObservableProperty]
    private string _vendorId = string.Empty;

    /// <summary>
    /// Gets or sets the device product identifier as a 4-character hexadecimal string (e.g., "5581").
    /// Placeholder property - will be populated in Epic 3 during Device Instance Path parsing.
    /// </summary>
    [ObservableProperty]
    private string _productId = string.Empty;

    /// <summary>
    /// Gets or sets the device serial number. May be null if not exposed by device.
    /// Placeholder property - will be populated in Epic 3 during Device Instance Path parsing.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSerialNumber))]
    private string? _serialNumber;

    /// <summary>
    /// Gets or sets the full Device Instance Path for parsing (e.g., "USB\VID_0781&amp;PID_5581\4C530001231120115142").
    /// Retrieved from the System.Devices.DeviceInstanceId property.
    /// </summary>
    [ObservableProperty]
    private string _deviceInstancePath = string.Empty;

    /// <summary>
    /// Gets or sets the HardwareIds array containing device identification strings.
    /// Contains entries like "USB\VID_0781&amp;PID_5581&amp;REV_0100" for VID/PID extraction.
    /// Retrieved from the System.Devices.HardwareIds property.
    /// </summary>
    [ObservableProperty]
    private string[] _hardwareIds = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the parent device instance path.
    /// For WPD-enumerated devices, this may contain the USB device path with VID/PID.
    /// Retrieved from the System.Devices.Parent property.
    /// </summary>
    [ObservableProperty]
    private string _parentDevicePath = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the device was successfully parsed.
    /// Placeholder property - will be set in Epic 3 after VID/PID/SerialNumber parsing completes.
    /// </summary>
    [ObservableProperty]
    private bool _isValid;

    /// <summary>
    /// Gets or sets parsing error details if device parsing failed.
    /// Placeholder property - will be populated in Epic 8 for error handling scenarios.
    /// </summary>
    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// Gets a value indicating whether the device has a serial number.
    /// Used for UI conditional logic (e.g., enabling/disabling copy buttons in the UI).
    /// This property automatically updates when <see cref="SerialNumber"/> changes.
    /// </summary>
    public bool HasSerialNumber => !string.IsNullOrWhiteSpace(SerialNumber);

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbDevice"/> class from a DeviceInformation object.
    /// </summary>
    /// <param name="deviceInfo">The Windows API DeviceInformation object containing device properties.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="deviceInfo"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// This constructor extracts basic properties (Id, Name, Manufacturer, DeviceInstancePath) from the
    /// provided <see cref="DeviceInformation"/> object. Extended properties are retrieved using the
    /// <see cref="Services.DeviceEnumerationService.GetPropertyValue{T}"/> helper method.
    /// </para>
    /// <para>
    /// Properties that depend on Device Instance Path parsing (VendorId, ProductId, SerialNumber)
    /// are initialized as placeholders and will be populated in Epic 3.
    /// </para>
    /// </remarks>
    public UsbDevice(DeviceInformation deviceInfo)
    {
        ArgumentNullException.ThrowIfNull(deviceInfo);

        // Extract basic properties from DeviceInformation
        _id = deviceInfo.Id;
        _friendlyName = deviceInfo.Name ?? "Unknown Device";

        // Extract extended properties using DeviceEnumerationService helper
        _manufacturer = Services.DeviceEnumerationService.GetPropertyValue<string>(
            deviceInfo,
            "System.Devices.Manufacturer"
        );

        _deviceInstancePath = Services.DeviceEnumerationService.GetPropertyValue<string>(
            deviceInfo,
            "System.Devices.DeviceInstanceId"
        ) ?? string.Empty;

        _hardwareIds = Services.DeviceEnumerationService.GetPropertyValue<string[]>(
            deviceInfo,
            "System.Devices.HardwareIds"
        ) ?? Array.Empty<string>();

        _parentDevicePath = Services.DeviceEnumerationService.GetPropertyValue<string>(
            deviceInfo,
            "System.Devices.Parent"
        ) ?? string.Empty;

        // Initialize placeholder properties (populated in Epic 3)
        _vendorId = string.Empty;
        _productId = string.Empty;
        _serialNumber = null;
        _isValid = false;
        _errorMessage = null;
    }
}
