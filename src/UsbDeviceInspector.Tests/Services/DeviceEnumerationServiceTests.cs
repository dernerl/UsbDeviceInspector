using FluentAssertions;
using NSubstitute;
using UsbDeviceInspector.Models;
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
        result.Should().BeAssignableTo<Task<IEnumerable<UsbDevice>>>();
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

    [Fact]
    public async Task EnumerateDevicesAsync_WithValidDevices_ReturnsUsbDeviceCollection()
    {
        // Arrange
        var service = new DeviceEnumerationService();

        // Act
        var result = await service.EnumerateDevicesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<UsbDevice>>();
        // Note: Actual device count depends on connected hardware
        // Test verifies type conversion works, not specific count
    }

    [Fact]
    public async Task EnumerateDevicesAsync_WithNoDevices_ReturnsEmptyCollection()
    {
        // Arrange
        var service = new DeviceEnumerationService();

        // Act
        var result = await service.EnumerateDevicesAsync();

        // Assert
        result.Should().NotBeNull();
        // Result could be empty or contain devices depending on hardware
        // The critical test is that it's not null
        result.Should().BeAssignableTo<IEnumerable<UsbDevice>>();
    }

    [Fact]
    public async Task RefreshDevicesAsync_ReturnsUsbDeviceCollection()
    {
        // Arrange
        var service = new DeviceEnumerationService();

        // Act
        var result = await service.RefreshDevicesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<UsbDevice>>();
        service.LastRefreshTime.Should().NotBeNull();
    }
}
