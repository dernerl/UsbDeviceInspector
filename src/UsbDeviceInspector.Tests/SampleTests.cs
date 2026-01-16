using FluentAssertions;
using Xunit;

namespace UsbDeviceInspector.Tests;

public class SampleTests
{
    [Fact]
    public void SampleTest_WhenTrue_ReturnsTrue()
    {
        // Arrange
        var expected = true;

        // Act
        var result = true;

        // Assert
        result.Should().Be(expected);
    }
}
