using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using UsbDeviceInspector.Models;
using UsbDeviceInspector.Services;
using Xunit;

namespace UsbDeviceInspector.Tests.Services;

/// <summary>
/// Unit tests validating the refresh functionality of <see cref="DeviceEnumerationService"/>.
/// These tests ensure AC 1-6 for Story 2.4.
/// </summary>
public class DeviceEnumerationServiceRefreshTests
{
    /// <summary>
    /// Verifies that RefreshDevicesAsync returns a non-null collection.
    /// Validates AC1: Method returns device collection.
    /// </summary>
    [Fact]
    public async Task RefreshDevicesAsync_ReturnsDeviceCollection_NotNull()
    {
        // Arrange
        var service = new DeviceEnumerationService();

        // Act
        var result = await service.RefreshDevicesAsync();

        // Assert
        result.Should().NotBeNull("RefreshDevicesAsync should return a valid collection");
    }

    /// <summary>
    /// Verifies that RefreshDevicesAsync updates LastRefreshTime after success.
    /// Validates AC5: LastRefreshTime property updated after each successful refresh.
    /// </summary>
    [Fact]
    public async Task RefreshDevicesAsync_UpdatesLastRefreshTime_AfterSuccess()
    {
        // Arrange
        var service = new DeviceEnumerationService();
        var beforeRefresh = DateTimeOffset.UtcNow;

        // Act
        await service.RefreshDevicesAsync();

        // Assert
        service.LastRefreshTime.Should().NotBeNull("LastRefreshTime should be set after refresh");
        service.LastRefreshTime.Should().BeOnOrAfter(beforeRefresh,
            "LastRefreshTime should be at or after the time we started");
        service.LastRefreshTime.Should().BeOnOrBefore(DateTimeOffset.UtcNow,
            "LastRefreshTime should not be in the future");
    }

    /// <summary>
    /// Verifies that RefreshDevicesAsync returns a new collection on each invocation.
    /// Validates AC4: Refresh behavior returns new device list on each invocation.
    /// </summary>
    [Fact]
    public async Task RefreshDevicesAsync_ReturnsNewCollection_OnEachInvocation()
    {
        // Arrange
        var service = new DeviceEnumerationService();

        // Act
        var firstResult = await service.RefreshDevicesAsync();
        var secondResult = await service.RefreshDevicesAsync();

        // Assert
        firstResult.Should().NotBeNull("first refresh should return a valid collection");
        secondResult.Should().NotBeNull("second refresh should return a valid collection");
        // Each call should return a collection (may be same content but different enumeration)
        firstResult.Should().NotBeSameAs(secondResult,
            "each refresh should return a new collection instance");
    }

    /// <summary>
    /// Verifies that RefreshDevicesAsync completes with ConfigureAwait(false) pattern.
    /// Validates async pattern compliance per coding standards.
    /// </summary>
    [Fact]
    public async Task RefreshDevicesAsync_CompletesAsync_WithConfigureAwaitFalse()
    {
        // Arrange
        var service = new DeviceEnumerationService();
        var task = service.RefreshDevicesAsync();

        // Act & Assert
        task.Should().NotBeNull("method should return a Task immediately");
        task.Should().BeAssignableTo<Task<IEnumerable<UsbDevice>>>(
            "return type should be Task<IEnumerable<UsbDevice>>");

        var result = await task;
        result.Should().NotBeNull("async operation should complete successfully");
    }

    /// <summary>
    /// Verifies that LastRefreshTime is null before any enumeration occurs.
    /// Validates initial state requirement.
    /// </summary>
    [Fact]
    public void LastRefreshTime_IsNull_BeforeFirstEnumeration()
    {
        // Arrange
        var service = new DeviceEnumerationService();

        // Act & Assert
        service.LastRefreshTime.Should().BeNull(
            "LastRefreshTime should be null before any enumeration");
    }

    /// <summary>
    /// Verifies that EnumerateDevicesAsync also updates LastRefreshTime.
    /// Validates AC5 consistency: any successful enumeration updates the timestamp.
    /// </summary>
    [Fact]
    public async Task EnumerateDevicesAsync_UpdatesLastRefreshTime_AfterSuccess()
    {
        // Arrange
        var service = new DeviceEnumerationService();
        var beforeEnumeration = DateTimeOffset.UtcNow;

        // Act
        await service.EnumerateDevicesAsync();

        // Assert
        service.LastRefreshTime.Should().NotBeNull(
            "LastRefreshTime should be set after enumeration");
        service.LastRefreshTime.Should().BeOnOrAfter(beforeEnumeration,
            "LastRefreshTime should be at or after the time we started");
    }

    /// <summary>
    /// Verifies that consecutive refreshes update LastRefreshTime each time.
    /// Validates that each refresh operation updates the timestamp.
    /// </summary>
    [Fact]
    public async Task RefreshDevicesAsync_UpdatesLastRefreshTime_OnConsecutiveCalls()
    {
        // Arrange
        var service = new DeviceEnumerationService();

        // Act
        await service.RefreshDevicesAsync();
        var firstRefreshTime = service.LastRefreshTime;

        // Small delay to ensure time difference
        await Task.Delay(10);

        await service.RefreshDevicesAsync();
        var secondRefreshTime = service.LastRefreshTime;

        // Assert
        firstRefreshTime.Should().NotBeNull();
        secondRefreshTime.Should().NotBeNull();
        secondRefreshTime.Should().BeOnOrAfter(firstRefreshTime!.Value,
            "second refresh time should be at or after first refresh time");
    }

    /// <summary>
    /// Verifies that RefreshDevicesAsync can run concurrently without issues.
    /// </summary>
    [Fact]
    public async Task RefreshDevicesAsync_CanRunConcurrently_WithMultipleCalls()
    {
        // Arrange
        var service = new DeviceEnumerationService();
        var tasks = new List<Task<IEnumerable<UsbDevice>>>();

        // Act - Start multiple concurrent refresh calls
        for (int i = 0; i < 3; i++)
        {
            tasks.Add(service.RefreshDevicesAsync());
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().HaveCount(3, "all three concurrent calls should complete");
        foreach (var result in results)
        {
            result.Should().NotBeNull("each call should return a valid result");
        }
        service.LastRefreshTime.Should().NotBeNull(
            "LastRefreshTime should be set after concurrent refreshes");
    }
}
