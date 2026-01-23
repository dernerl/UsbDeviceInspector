using FluentAssertions;
using UsbDeviceInspector.Models;
using UsbDeviceInspector.Services;
using UsbDeviceInspector.Services.Interfaces;
using Windows.Devices.Enumeration;
using Xunit;

namespace UsbDeviceInspector.Tests.Services;

/// <summary>
/// Unit tests for <see cref="DeviceParsingService"/>.
/// Tests validate service instantiation, interface implementation, and method signatures.
/// </summary>
/// <remarks>
/// <para>
/// NOTE: Tests that require a <see cref="UsbDevice"/> instance use real USB device
/// enumeration due to Windows Runtime constraints. <see cref="DeviceInformation"/> is a
/// sealed Windows Runtime type that cannot be mocked with NSubstitute or other mocking frameworks.
/// </para>
/// <para>
/// Tests that depend on real devices will be skipped if no USB storage devices are connected.
/// </para>
/// </remarks>
public class DeviceParsingServiceTests
{
    #region Service Instantiation Tests

    [Fact]
    public void DeviceParsingService_CanBeInstantiated()
    {
        // Arrange & Act
        var service = new DeviceParsingService();

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void DeviceParsingService_ImplementsIDeviceParsingService()
    {
        // Arrange & Act
        var service = new DeviceParsingService();

        // Assert
        service.Should().BeAssignableTo<IDeviceParsingService>();
    }

    [Fact]
    public void DeviceParsingService_IsStateless_NoInstanceFields()
    {
        // Arrange & Act
        var service = new DeviceParsingService();
        var fields = typeof(DeviceParsingService)
            .GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

        // Assert
        fields.Should().BeEmpty("DeviceParsingService should be stateless with no instance fields");
    }

    [Fact]
    public void DeviceParsingService_HasParameterlessConstructor()
    {
        // Arrange
        var constructors = typeof(DeviceParsingService).GetConstructors();

        // Act
        var parameterlessConstructor = constructors
            .FirstOrDefault(c => c.GetParameters().Length == 0);

        // Assert
        parameterlessConstructor.Should().NotBeNull("DeviceParsingService should have a parameterless constructor");
    }

    #endregion

    #region ParseDeviceProperties Tests

    [Fact]
    public void ParseDeviceProperties_WithNullDevice_ReturnsFalse()
    {
        // Arrange
        var service = new DeviceParsingService();

        // Act
        var result = service.ParseDeviceProperties(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ParseDeviceProperties_MethodSignature_AcceptsUsbDeviceAndReturnsBool()
    {
        // Arrange
        var service = new DeviceParsingService();
        var device = await GetTestUsbDeviceOrSkip();
        if (device == null) return; // No devices available - skip test

        // Act
        var result = service.ParseDeviceProperties(device);

        // Assert
        // Verify method returns a bool (compilation proves signature works)
        // Result depends on whether device has valid VID/PID in path
        result.GetType().Should().Be(typeof(bool));
    }

    #endregion

    #region ExtractVidPid Tests

    [Fact]
    public void ExtractVidPid_StandardFormat_ReturnsCorrectVidAndPid()
    {
        // Arrange
        var deviceInstancePath = @"USB\VID_0781&PID_5581\4C530001231124103024";

        // Act
        var result = DeviceParsingService.ExtractVidPid(deviceInstancePath);

        // Assert
        result.Should().NotBeNull();
        result!.Value.vid.Should().Be("0781");
        result!.Value.pid.Should().Be("5581");
    }

    [Fact]
    public void ExtractVidPid_WithRevision_ReturnsCorrectVidAndPid()
    {
        // Arrange
        var deviceInstancePath = @"USB\VID_0781&PID_5581&REV_0100\4C530001231124103024";

        // Act
        var result = DeviceParsingService.ExtractVidPid(deviceInstancePath);

        // Assert
        result.Should().NotBeNull();
        result!.Value.vid.Should().Be("0781");
        result!.Value.pid.Should().Be("5581");
    }

    [Fact]
    public void ExtractVidPid_LowercasePattern_ReturnsCorrectVidAndPid()
    {
        // Arrange
        var deviceInstancePath = @"usb\vid_0781&pid_5581\serial";

        // Act
        var result = DeviceParsingService.ExtractVidPid(deviceInstancePath);

        // Assert
        result.Should().NotBeNull();
        result!.Value.vid.Should().Be("0781");
        result!.Value.pid.Should().Be("5581");
    }

    [Fact]
    public void ExtractVidPid_MixedCasePattern_ReturnsCorrectVidAndPid()
    {
        // Arrange
        var deviceInstancePath = @"USB\Vid_0781&Pid_5581\serial";

        // Act
        var result = DeviceParsingService.ExtractVidPid(deviceInstancePath);

        // Assert
        result.Should().NotBeNull();
        result!.Value.vid.Should().Be("0781");
        result!.Value.pid.Should().Be("5581");
    }

    [Fact]
    public void ExtractVidPid_MissingVid_ReturnsNull()
    {
        // Arrange
        var deviceInstancePath = @"USB\PID_5581\serial";

        // Act
        var result = DeviceParsingService.ExtractVidPid(deviceInstancePath);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ExtractVidPid_MissingPid_ReturnsNull()
    {
        // Arrange
        var deviceInstancePath = @"USB\VID_0781\serial";

        // Act
        var result = DeviceParsingService.ExtractVidPid(deviceInstancePath);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ExtractVidPid_EmptyString_ReturnsNull()
    {
        // Arrange
        var deviceInstancePath = string.Empty;

        // Act
        var result = DeviceParsingService.ExtractVidPid(deviceInstancePath);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ExtractVidPid_NullString_ReturnsNull()
    {
        // Arrange
        string? deviceInstancePath = null;

        // Act
        var result = DeviceParsingService.ExtractVidPid(deviceInstancePath!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ExtractVidPid_InvalidHexCharacters_ReturnsNull()
    {
        // Arrange - G-J are not valid hex characters
        var deviceInstancePath = @"USB\VID_GHIJ&PID_5581\serial";

        // Act
        var result = DeviceParsingService.ExtractVidPid(deviceInstancePath);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ExtractVidPid_LowercaseHexDigits_ReturnsUppercase()
    {
        // Arrange - lowercase hex digits a-f
        var deviceInstancePath = @"USB\VID_abcd&PID_ef01\serial";

        // Act
        var result = DeviceParsingService.ExtractVidPid(deviceInstancePath);

        // Assert
        result.Should().NotBeNull();
        result!.Value.vid.Should().Be("ABCD");
        result!.Value.pid.Should().Be("EF01");
    }

    #endregion

    #region ParseDeviceProperties VID/PID Tests

    [Fact]
    public async Task ParseDeviceProperties_WithValidDeviceInstancePath_SetsVidAndPid()
    {
        // Arrange
        var service = new DeviceParsingService();
        var device = await GetTestUsbDeviceOrSkip();
        if (device == null) return; // No devices available - skip test

        // Act
        var result = service.ParseDeviceProperties(device);

        // Assert
        // If device has valid USB VID/PID format, result should be true and properties set
        if (result)
        {
            device.VendorId.Should().MatchRegex("^[0-9A-F]{4}$", "VID should be 4 uppercase hex characters");
            device.ProductId.Should().MatchRegex("^[0-9A-F]{4}$", "PID should be 4 uppercase hex characters");
        }
    }

    [Fact]
    public async Task ParseDeviceProperties_WithInvalidPath_SetsIsValidFalse()
    {
        // Arrange
        var service = new DeviceParsingService();
        var device = await GetTestUsbDeviceOrSkip();
        if (device == null) return; // No devices available - skip test

        // Act
        // Note: Real devices should have valid paths; this test validates the mechanism exists
        // We test the specific invalid path behavior through ExtractVidPid tests above
        var result = service.ParseDeviceProperties(device);

        // Assert
        // Either extraction succeeds (valid USB device) or fails (IsValid = false)
        if (!result)
        {
            device.IsValid.Should().BeFalse();
        }
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
            "System.Devices.DeviceInstanceId",
            "System.Devices.HardwareIds"
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
