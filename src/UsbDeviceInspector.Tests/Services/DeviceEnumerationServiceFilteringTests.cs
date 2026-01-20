using FluentAssertions;
using UsbDeviceInspector.Services;

namespace UsbDeviceInspector.Tests.Services;

/// <summary>
/// Unit tests for device filtering logic in DeviceEnumerationService.
/// Tests validate that USB storage devices are correctly identified and
/// internal SD card readers and non-USB devices are properly excluded.
/// </summary>
public class DeviceEnumerationServiceFilteringTests
{
    #region IsUsbDevicePath Tests

    [Fact]
    public void IsUsbDevicePath_WithUsbFlashDrivePath_ReturnsTrue()
    {
        // Arrange
        var deviceInstancePath = @"USB\VID_0781&PID_5581\4C530001231120115142";

        // Act
        var result = DeviceEnumerationService.IsUsbDevicePath(deviceInstancePath);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsUsbDevicePath_WithUsbExternalHddPath_ReturnsTrue()
    {
        // Arrange
        var deviceInstancePath = @"USB\VID_0480&PID_A009\20151234567890";

        // Act
        var result = DeviceEnumerationService.IsUsbDevicePath(deviceInstancePath);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsUsbDevicePath_WithUsbCardReaderPath_ReturnsTrue()
    {
        // Arrange - USB-connected card reader should be INCLUDED
        var deviceInstancePath = @"USB\VID_058F&PID_6362\ABCDEF123456";

        // Act
        var result = DeviceEnumerationService.IsUsbDevicePath(deviceInstancePath);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsUsbDevicePath_WithLowercaseUsbPrefix_ReturnsTrue()
    {
        // Arrange
        var deviceInstancePath = @"usb\VID_0781&PID_5581\4C530001231120115142";

        // Act
        var result = DeviceEnumerationService.IsUsbDevicePath(deviceInstancePath);

        // Assert
        result.Should().BeTrue("comparison should be case-insensitive");
    }

    [Fact]
    public void IsUsbDevicePath_WithScsiPrefix_ReturnsFalse()
    {
        // Arrange - Internal SCSI drive should be EXCLUDED
        var deviceInstancePath = @"SCSI\DISK&VEN_SAMSUNG&PROD_SSD_870_EVO\12345";

        // Act
        var result = DeviceEnumerationService.IsUsbDevicePath(deviceInstancePath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsUsbDevicePath_WithSataPrefix_ReturnsFalse()
    {
        // Arrange - Internal SATA drive should be EXCLUDED
        var deviceInstancePath = @"SATA\DISK&VEN_WDC&PROD_WD10EZEX\12345";

        // Act
        var result = DeviceEnumerationService.IsUsbDevicePath(deviceInstancePath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsUsbDevicePath_WithNvmePrefix_ReturnsFalse()
    {
        // Arrange - Internal NVMe SSD should be EXCLUDED
        var deviceInstancePath = @"NVME\SAMSUNG_980_PRO\12345678";

        // Act
        var result = DeviceEnumerationService.IsUsbDevicePath(deviceInstancePath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsUsbDevicePath_WithPciePrefix_ReturnsFalse()
    {
        // Arrange - Internal PCIe device should be EXCLUDED
        var deviceInstancePath = @"PCIE\VEN_144D&DEV_A808\12345";

        // Act
        var result = DeviceEnumerationService.IsUsbDevicePath(deviceInstancePath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsUsbDevicePath_WithIdePrefix_ReturnsFalse()
    {
        // Arrange - Internal IDE drive should be EXCLUDED
        var deviceInstancePath = @"IDE\DISKWDC_WD800JB\12345";

        // Act
        var result = DeviceEnumerationService.IsUsbDevicePath(deviceInstancePath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsUsbDevicePath_WithNullPath_ReturnsFalse()
    {
        // Arrange
        string? deviceInstancePath = null;

        // Act
        var result = DeviceEnumerationService.IsUsbDevicePath(deviceInstancePath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsUsbDevicePath_WithEmptyPath_ReturnsFalse()
    {
        // Arrange
        var deviceInstancePath = string.Empty;

        // Act
        var result = DeviceEnumerationService.IsUsbDevicePath(deviceInstancePath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsUsbDevicePath_WithUnknownPrefix_ReturnsFalse()
    {
        // Arrange - Unknown prefix should be EXCLUDED (not USB)
        var deviceInstancePath = @"UNKNOWN\DEVICE\12345";

        // Act
        var result = DeviceEnumerationService.IsUsbDevicePath(deviceInstancePath);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region IsInternalSdCardReaderPath Tests

    [Fact]
    public void IsInternalSdCardReaderPath_WithSdPrefix_ReturnsTrue()
    {
        // Arrange - Internal SD card reader should be EXCLUDED
        var deviceInstancePath = @"SD\DISK&VEN_&PROD_SD_CARD\12345";

        // Act
        var result = DeviceEnumerationService.IsInternalSdCardReaderPath(deviceInstancePath);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsInternalSdCardReaderPath_WithSdbusPrefix_ReturnsTrue()
    {
        // Arrange - Internal SDBUS device should be EXCLUDED
        var deviceInstancePath = @"SDBUS\SD\VID_12&OID_34&NAME_SD\0";

        // Act
        var result = DeviceEnumerationService.IsInternalSdCardReaderPath(deviceInstancePath);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsInternalSdCardReaderPath_WithMmcPrefix_ReturnsTrue()
    {
        // Arrange - Internal MMC card reader should be EXCLUDED
        var deviceInstancePath = @"MMC\DISK&VEN_MMC&PROD_CARD\12345";

        // Act
        var result = DeviceEnumerationService.IsInternalSdCardReaderPath(deviceInstancePath);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsInternalSdCardReaderPath_WithLowercaseSdPrefix_ReturnsTrue()
    {
        // Arrange
        var deviceInstancePath = @"sd\DISK&VEN_&PROD_SD_CARD\12345";

        // Act
        var result = DeviceEnumerationService.IsInternalSdCardReaderPath(deviceInstancePath);

        // Assert
        result.Should().BeTrue("comparison should be case-insensitive");
    }

    [Fact]
    public void IsInternalSdCardReaderPath_WithUsbPrefix_ReturnsFalse()
    {
        // Arrange - USB-connected card reader should NOT be flagged as internal SD
        var deviceInstancePath = @"USB\VID_058F&PID_6362\ABCDEF123456";

        // Act
        var result = DeviceEnumerationService.IsInternalSdCardReaderPath(deviceInstancePath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsInternalSdCardReaderPath_WithNullPath_ReturnsFalse()
    {
        // Arrange
        string? deviceInstancePath = null;

        // Act
        var result = DeviceEnumerationService.IsInternalSdCardReaderPath(deviceInstancePath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsInternalSdCardReaderPath_WithEmptyPath_ReturnsFalse()
    {
        // Arrange
        var deviceInstancePath = string.Empty;

        // Act
        var result = DeviceEnumerationService.IsInternalSdCardReaderPath(deviceInstancePath);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetDeviceSelector Tests

    [Fact]
    public void GetDeviceSelector_WhenCalled_ReturnsNonNullString()
    {
        // Act
        var selector = DeviceEnumerationService.GetDeviceSelector();

        // Assert
        selector.Should().NotBeNullOrEmpty();
    }

    #endregion
}
