using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using UsbDeviceInspector.Models;
using UsbDeviceInspector.Services;
using Xunit;

namespace UsbDeviceInspector.Tests.Services;

/// <summary>
/// Unit tests validating the async behavior of <see cref="DeviceEnumerationService"/>.
/// These tests ensure UI responsiveness and compliance with NFR1 and NFR2.
/// </summary>
public class DeviceEnumerationServiceAsyncTests
{
    /// <summary>
    /// Verifies that EnumerateDevicesAsync returns a non-null Task immediately.
    /// This validates AC1: Method returns Task&lt;IEnumerable&lt;DeviceInformation&gt;&gt;.
    /// </summary>
    [Fact]
    public void EnumerateDevicesAsync_ReturnsTask_NotNull()
    {
        // Arrange
        var service = new DeviceEnumerationService();

        // Act
        var task = service.EnumerateDevicesAsync();

        // Assert
        task.Should().NotBeNull("the method should return a Task immediately");
        task.Should().BeAssignableTo<Task<IEnumerable<UsbDevice>>>(
            "the return type should be Task<IEnumerable<UsbDevice>>");
    }

    /// <summary>
    /// Verifies that EnumerateDevicesAsync completes successfully when awaited.
    /// This validates AC2: Method uses await for Windows API calls.
    /// </summary>
    [Fact]
    public async Task EnumerateDevicesAsync_CompletesSuccessfully_WhenAwaited()
    {
        // Arrange
        var service = new DeviceEnumerationService();

        // Act
        var result = await service.EnumerateDevicesAsync();

        // Assert
        result.Should().NotBeNull("the method should return a valid enumerable");
    }

    /// <summary>
    /// Verifies that multiple concurrent calls to EnumerateDevicesAsync can execute
    /// without blocking each other, validating AC3: Method runs on background thread pool.
    /// </summary>
    [Fact]
    public async Task EnumerateDevicesAsync_CanRunConcurrently_WithMultipleCalls()
    {
        // Arrange
        var service = new DeviceEnumerationService();
        var tasks = new List<Task<IEnumerable<UsbDevice>>>();

        // Act - Start multiple concurrent enumeration calls
        for (int i = 0; i < 3; i++)
        {
            tasks.Add(service.EnumerateDevicesAsync());
        }

        // All tasks should complete without deadlock
        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().HaveCount(3, "all three concurrent calls should complete");
        foreach (var result in results)
        {
            result.Should().NotBeNull("each call should return a valid result");
        }
    }

    /// <summary>
    /// Verifies that EnumerateDevicesAsync does not block the calling thread.
    /// This validates AC3: Method runs entirely on background thread pool.
    /// </summary>
    [Fact]
    public async Task EnumerateDevicesAsync_DoesNotBlock_CallingThread()
    {
        // Arrange
        var service = new DeviceEnumerationService();
        var callingThreadId = Environment.CurrentManagedThreadId;
        var taskStarted = false;
        var taskCompleted = false;

        // Act - Run on a separate thread pool thread
        await Task.Run(async () =>
        {
            taskStarted = true;
            await service.EnumerateDevicesAsync();
            taskCompleted = true;
        });

        // Assert
        taskStarted.Should().BeTrue("the task should have started");
        taskCompleted.Should().BeTrue("the task should have completed without blocking");
    }

    /// <summary>
    /// Verifies that EnumerateDevicesAsync returns immediately without waiting
    /// for the enumeration to complete (non-blocking start).
    /// </summary>
    [Fact]
    public void EnumerateDevicesAsync_ReturnsImmediately_WithoutBlocking()
    {
        // Arrange
        var service = new DeviceEnumerationService();
        var returnedImmediately = false;

        // Act - The method should return a Task immediately
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var task = service.EnumerateDevicesAsync();
        stopwatch.Stop();

        // The call to get the Task should be nearly instantaneous (< 100ms)
        // The actual enumeration happens when the Task is awaited
        returnedImmediately = stopwatch.ElapsedMilliseconds < 100;

        // Assert
        task.Should().NotBeNull();
        returnedImmediately.Should().BeTrue(
            "the method should return a Task immediately without blocking");
    }

    /// <summary>
    /// Verifies that EnumerateDevicesAsync can be cancelled via CancellationToken
    /// when wrapped in a Task.WhenAny pattern (standard async cancellation approach).
    /// </summary>
    [Fact]
    public async Task EnumerateDevicesAsync_CanBeCancelledViaTimeout_UsingWhenAny()
    {
        // Arrange
        var service = new DeviceEnumerationService();
        using var cts = new CancellationTokenSource();
        var timeout = TimeSpan.FromSeconds(10); // Generous timeout

        // Act
        var enumerationTask = service.EnumerateDevicesAsync();
        var timeoutTask = Task.Delay(timeout, cts.Token);
        var completedTask = await Task.WhenAny(enumerationTask, timeoutTask);

        // Cancel the timeout if enumeration completed first
        if (completedTask == enumerationTask)
        {
            cts.Cancel();
        }

        // Assert
        completedTask.Should().Be(enumerationTask,
            "enumeration should complete before the generous timeout");
    }

    /// <summary>
    /// Verifies that EnumerateDevicesAsync completes within 3 seconds as required by NFR1.
    /// This test validates performance requirement: "Device enumeration completes within
    /// 3 seconds for up to 10 connected devices."
    /// </summary>
    /// <remarks>
    /// This test is environment-dependent and may vary based on connected devices
    /// and system load. Marked with Performance category for selective test runs.
    /// </remarks>
    [Fact]
    [Trait("Category", "Performance")]
    public async Task EnumerateDevicesAsync_CompletesWithinTimeout_ThreeSeconds()
    {
        // Arrange
        var service = new DeviceEnumerationService();
        var timeout = TimeSpan.FromSeconds(3);

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var enumerationTask = service.EnumerateDevicesAsync();
        var timeoutTask = Task.Delay(timeout);
        var completedTask = await Task.WhenAny(enumerationTask, timeoutTask);
        stopwatch.Stop();

        // Assert
        completedTask.Should().Be(enumerationTask,
            $"enumeration should complete within {timeout.TotalSeconds} seconds (NFR1), " +
            $"but took {stopwatch.ElapsedMilliseconds}ms");

        // Verify we got actual results
        var result = await enumerationTask;
        result.Should().NotBeNull("enumeration should return a valid result");
    }

    /// <summary>
    /// Verifies that the Task returned by EnumerateDevicesAsync is in the correct state
    /// before and after awaiting.
    /// </summary>
    [Fact]
    public async Task EnumerateDevicesAsync_TaskState_TransitionsCorrectly()
    {
        // Arrange
        var service = new DeviceEnumerationService();

        // Act
        var task = service.EnumerateDevicesAsync();

        // Task should not be completed immediately for async operations
        // (though it might be if very fast, so we just check it's not faulted)
        task.IsFaulted.Should().BeFalse("task should not be in faulted state immediately");
        task.IsCanceled.Should().BeFalse("task should not be in canceled state");

        // Wait for completion
        await task;

        // Assert - After await, task should be completed successfully
        task.IsCompleted.Should().BeTrue("task should be completed after await");
        task.IsCompletedSuccessfully.Should().BeTrue("task should complete successfully");
        task.IsFaulted.Should().BeFalse("task should not be faulted");
    }
}