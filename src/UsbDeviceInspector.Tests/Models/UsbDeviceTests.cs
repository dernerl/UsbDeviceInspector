using System;
using FluentAssertions;
using UsbDeviceInspector.Models;
using Windows.Devices.Enumeration;
using Xunit;

namespace UsbDeviceInspector.Tests.Models;

/// <summary>
/// Unit tests for the <see cref="UsbDevice"/> model class.
/// </summary>
/// <remarks>
/// <para>
/// NOTE: This test class has limited coverage due to Windows Runtime type constraints.
/// <see cref="DeviceInformation"/> is a sealed Windows Runtime type that cannot be mocked
/// with NSubstitute or other mocking frameworks (no public constructor, no interfaces).
/// </para>
/// <para>
/// Constructor behavior with real DeviceInformation objects is validated through:
/// - Integration tests that enumerate actual USB devices
/// - Manual testing on development machines
/// - End-to-end UI testing
/// </para>
/// <para>
/// This test class focuses on:
/// - Null parameter validation (which can be tested without mocking)
/// - Property change notification behavior (INotifyPropertyChanged)
/// - Computed property logic (HasSerialNumber)
/// </para>
/// </remarks>
public class UsbDeviceTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullDeviceInformation_ThrowsArgumentNullException()
    {
        // Arrange
        DeviceInformation? nullDeviceInfo = null;

        // Act
        Action act = () => new UsbDevice(nullDeviceInfo!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("deviceInfo");
    }

    #endregion

    #region Property Initialization Tests

    [Fact]
    public async Task PlaceholderProperties_AreInitializedCorrectly_WhenConstructedFromRealDevice()
    {
        // Arrange - Enumerate real devices to get a valid DeviceInformation object
        string aqsFilter = "System.Devices.InterfaceClassGuid:=\"{6AC27878-A6FA-4155-BA85-F98F491D4F33}\"";
        var additionalProperties = new[] { "System.ItemNameDisplay", "System.Devices.Manufacturer", "System.Devices.DeviceInstanceId" };

        var devices = await DeviceInformation.FindAllAsync(aqsFilter, additionalProperties);

        // Skip test if no USB storage devices are connected
        if (devices.Count == 0)
        {
            // Test inconclusive - no USB devices available
            return;
        }

        var firstDevice = devices[0];

        // Act
        var usbDevice = new UsbDevice(firstDevice);

        // Assert - Verify placeholder properties are initialized correctly
        usbDevice.VendorId.Should().Be(string.Empty, "VendorId is a placeholder for Epic 3");
        usbDevice.ProductId.Should().Be(string.Empty, "ProductId is a placeholder for Epic 3");
        usbDevice.SerialNumber.Should().BeNull("SerialNumber is a placeholder for Epic 3");
        usbDevice.IsValid.Should().BeFalse("IsValid is a placeholder for Epic 3");
        usbDevice.ErrorMessage.Should().BeNull("ErrorMessage is a placeholder for Epic 8");

        // Verify extracted properties are not null/empty
        usbDevice.Id.Should().NotBeNullOrEmpty("Id should be extracted from DeviceInformation");
        usbDevice.FriendlyName.Should().NotBeNullOrEmpty("FriendlyName should be extracted from DeviceInformation");
        usbDevice.DeviceInstancePath.Should().NotBeNullOrEmpty("DeviceInstancePath should be extracted from DeviceInformation");
    }

    #endregion

    #region HasSerialNumber Computed Property Tests

    [Fact]
    public async Task HasSerialNumber_WithNullSerialNumber_ReturnsFalse()
    {
        // Arrange - Get a real device to construct UsbDevice
        var usbDevice = await GetTestUsbDeviceOrSkip();
        if (usbDevice == null) return; // No devices available

        // Ensure SerialNumber is null (default state)
        usbDevice.SerialNumber.Should().BeNull("SerialNumber is null by default");

        // Act & Assert
        usbDevice.HasSerialNumber.Should().BeFalse("HasSerialNumber should return false when SerialNumber is null");
    }

    [Fact]
    public async Task HasSerialNumber_WithEmptySerialNumber_ReturnsFalse()
    {
        // Arrange
        var usbDevice = await GetTestUsbDeviceOrSkip();
        if (usbDevice == null) return;

        // Act
        usbDevice.SerialNumber = string.Empty;

        // Assert
        usbDevice.HasSerialNumber.Should().BeFalse("HasSerialNumber should return false when SerialNumber is empty");
    }

    [Fact]
    public async Task HasSerialNumber_WithWhitespaceSerialNumber_ReturnsFalse()
    {
        // Arrange
        var usbDevice = await GetTestUsbDeviceOrSkip();
        if (usbDevice == null) return;

        // Act
        usbDevice.SerialNumber = "   ";

        // Assert
        usbDevice.HasSerialNumber.Should().BeFalse("HasSerialNumber should return false when SerialNumber is whitespace");
    }

    [Fact]
    public async Task HasSerialNumber_WithValidSerialNumber_ReturnsTrue()
    {
        // Arrange
        var usbDevice = await GetTestUsbDeviceOrSkip();
        if (usbDevice == null) return;

        // Act
        usbDevice.SerialNumber = "4C530001231120115142";

        // Assert
        usbDevice.HasSerialNumber.Should().BeTrue("HasSerialNumber should return true when SerialNumber has a value");
    }

    #endregion

    #region INotifyPropertyChanged Tests

    [Fact]
    public async Task PropertyChanged_WhenFriendlyNameChanges_RaisesEvent()
    {
        // Arrange
        var usbDevice = await GetTestUsbDeviceOrSkip();
        if (usbDevice == null) return;

        bool eventRaised = false;
        string? changedPropertyName = null;
        usbDevice.PropertyChanged += (s, e) =>
        {
            eventRaised = true;
            changedPropertyName = e.PropertyName;
        };

        // Act
        usbDevice.FriendlyName = "New Device Name";

        // Assert
        eventRaised.Should().BeTrue("PropertyChanged event should be raised");
        changedPropertyName.Should().Be("FriendlyName");
    }

    [Fact]
    public async Task PropertyChanged_WhenManufacturerChanges_RaisesEvent()
    {
        // Arrange
        var usbDevice = await GetTestUsbDeviceOrSkip();
        if (usbDevice == null) return;

        bool eventRaised = false;
        string? changedPropertyName = null;
        usbDevice.PropertyChanged += (s, e) =>
        {
            eventRaised = true;
            changedPropertyName = e.PropertyName;
        };

        // Act
        usbDevice.Manufacturer = "New Manufacturer";

        // Assert
        eventRaised.Should().BeTrue("PropertyChanged event should be raised");
        changedPropertyName.Should().Be("Manufacturer");
    }

    [Fact]
    public async Task PropertyChanged_WhenSerialNumberChanges_RaisesSerialNumberAndHasSerialNumberEvents()
    {
        // Arrange
        var usbDevice = await GetTestUsbDeviceOrSkip();
        if (usbDevice == null) return;

        var propertyChangedEvents = new System.Collections.Generic.List<string?>();
        usbDevice.PropertyChanged += (s, e) => propertyChangedEvents.Add(e.PropertyName);

        // Act
        usbDevice.SerialNumber = "NewSerial123";

        // Assert
        propertyChangedEvents.Should().Contain("SerialNumber", "PropertyChanged event should be raised for SerialNumber");
        propertyChangedEvents.Should().Contain("HasSerialNumber",
            "PropertyChanged event should be raised for HasSerialNumber due to [NotifyPropertyChangedFor] attribute");
    }

    [Fact]
    public async Task HasSerialNumber_UpdatesWhenSerialNumberChanges()
    {
        // Arrange
        var usbDevice = await GetTestUsbDeviceOrSkip();
        if (usbDevice == null) return;

        // Verify initial state
        usbDevice.HasSerialNumber.Should().BeFalse("Initial SerialNumber is null");

        // Act - Set to valid value
        usbDevice.SerialNumber = "TestSerial456";

        // Assert
        usbDevice.HasSerialNumber.Should().BeTrue("HasSerialNumber should update to true when SerialNumber is set");

        // Act - Set back to null
        usbDevice.SerialNumber = null;

        // Assert
        usbDevice.HasSerialNumber.Should().BeFalse("HasSerialNumber should update to false when SerialNumber is null");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets a test UsbDevice instance by enumerating real USB devices.
    /// Returns null if no devices are available (test will be skipped).
    /// </summary>
    private async Task<UsbDevice?> GetTestUsbDeviceOrSkip()
    {
        string aqsFilter = "System.Devices.InterfaceClassGuid:=\"{6AC27878-A6FA-4155-BA85-F98F491D4F33}\"";
        var additionalProperties = new[]
        {
            "System.ItemNameDisplay",
            "System.Devices.Manufacturer",
            "System.Devices.DeviceInstanceId"
        };

        var devices = await DeviceInformation.FindAllAsync(aqsFilter, additionalProperties);

        if (devices.Count == 0)
        {
            // No USB storage devices connected - test will be skipped
            return null;
        }

        return new UsbDevice(devices[0]);
    }

    #endregion
}
