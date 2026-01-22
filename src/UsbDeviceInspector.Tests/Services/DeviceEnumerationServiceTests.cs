using FluentAssertions;
using NSubstitute;
using UsbDeviceInspector.Services;
using UsbDeviceInspector.Services.Interfaces;
using Windows.Devices.Enumeration;

namespace UsbDeviceInspector.Tests.Services;

public class DeviceEnumerationServiceTests
{
    [Fact]
    public void Constructor_WhenCalled_CreatesInstance()
    {
        // Arrange & Act
        var service = new DeviceEnumerationService();

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Service_ImplementsInterface()
    {
        // Arrange & Act
        var service = new DeviceEnumerationService();

        // Assert
        service.Should().BeAssignableTo<IDeviceEnumerationService>();
    }

    [Fact]
    public void EnumerateDevicesAsync_WhenCalled_ReturnsTask()
    {
        // Arrange
        var service = new DeviceEnumerationService();

        // Act
        var result = service.EnumerateDevicesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<Task<IEnumerable<DeviceInformation>>>();
    }

    [Fact]
    public void GetPropertyValue_WithNullDevice_ReturnsDefault()
    {
        // Arrange
        DeviceInformation? nullDevice = null;

        // Act
        var result = DeviceEnumerationService.GetPropertyValue<string>(nullDevice, "System.ItemNameDisplay");

        // Assert
        result.Should().BeNull();
    }
}
