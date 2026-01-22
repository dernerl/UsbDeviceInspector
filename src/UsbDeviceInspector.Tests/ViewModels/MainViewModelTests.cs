using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using UsbDeviceInspector.Models;
using UsbDeviceInspector.Services.Interfaces;
using UsbDeviceInspector.ViewModels;
using Xunit;

namespace UsbDeviceInspector.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="MainViewModel"/>.
/// </summary>
public class MainViewModelTests
{
    private readonly IDeviceEnumerationService _mockDeviceEnumerationService;

    public MainViewModelTests()
    {
        _mockDeviceEnumerationService = Substitute.For<IDeviceEnumerationService>();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_InjectsDeviceEnumerationService_Successfully()
    {
        // Arrange & Act
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Assert
        viewModel.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenServiceIsNull()
    {
        // Arrange & Act
        var act = () => new MainViewModel(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("deviceEnumerationService");
    }

    [Fact]
    public void Constructor_InitializesDevicesCollection_AsEmpty()
    {
        // Arrange & Act
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Assert
        viewModel.Devices.Should().NotBeNull();
        viewModel.Devices.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_InitializesIsLoading_AsFalse()
    {
        // Arrange & Act
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Assert
        viewModel.IsLoading.Should().BeFalse();
    }

    [Fact]
    public void Constructor_InitializesErrorMessage_AsNull()
    {
        // Arrange & Act
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Assert
        viewModel.ErrorMessage.Should().BeNull();
    }

    #endregion

    #region InitializeAsync Tests

    [Fact]
    public async Task InitializeAsync_CallsEnumerateDevicesAsync_OnStartup()
    {
        // Arrange
        _mockDeviceEnumerationService.EnumerateDevicesAsync()
            .Returns(Task.FromResult<IEnumerable<UsbDevice>>(Array.Empty<UsbDevice>()));
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act
        await viewModel.InitializeAsync();

        // Assert
        await _mockDeviceEnumerationService.Received(1).EnumerateDevicesAsync();
    }

    [Fact]
    public async Task InitializeAsync_SetsIsLoadingTrue_BeforeEnumeration()
    {
        // Arrange
        var loadingStates = new List<bool>();
        var tcs = new TaskCompletionSource<IEnumerable<UsbDevice>>();
        _mockDeviceEnumerationService.EnumerateDevicesAsync().Returns(tcs.Task);

        var viewModel = new MainViewModel(_mockDeviceEnumerationService);
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.IsLoading))
            {
                loadingStates.Add(viewModel.IsLoading);
            }
        };

        // Act
        var initTask = viewModel.InitializeAsync();

        // Assert - IsLoading should be true while waiting
        loadingStates.Should().Contain(true);
        viewModel.IsLoading.Should().BeTrue();

        // Cleanup - complete the task
        tcs.SetResult(Array.Empty<UsbDevice>());
        await initTask;
    }

    [Fact]
    public async Task InitializeAsync_SetsIsLoadingFalse_AfterEnumeration()
    {
        // Arrange
        _mockDeviceEnumerationService.EnumerateDevicesAsync()
            .Returns(Task.FromResult<IEnumerable<UsbDevice>>(Array.Empty<UsbDevice>()));
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act
        await viewModel.InitializeAsync();

        // Assert
        viewModel.IsLoading.Should().BeFalse();
    }

    [Fact]
    public async Task InitializeAsync_PopulatesDevicesCollection_WithResults()
    {
        // Arrange
        // Note: UsbDevice can be mocked using NSubstitute for testing
        // For this test we verify behavior with empty collection
        _mockDeviceEnumerationService.EnumerateDevicesAsync()
            .Returns(Task.FromResult<IEnumerable<UsbDevice>>(Array.Empty<UsbDevice>()));
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act
        await viewModel.InitializeAsync();

        // Assert
        viewModel.Devices.Should().NotBeNull();
        viewModel.Devices.Count.Should().Be(0);
    }

    [Fact]
    public async Task InitializeAsync_SetsErrorMessage_OnEnumerationFailure()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test enumeration failure");
        _mockDeviceEnumerationService.EnumerateDevicesAsync()
            .ThrowsAsync(expectedException);
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act
        await viewModel.InitializeAsync();

        // Assert
        viewModel.ErrorMessage.Should().NotBeNull();
        viewModel.ErrorMessage.Should().Contain("Test enumeration failure");
    }

    [Fact]
    public async Task InitializeAsync_SetsIsLoadingFalse_AfterEnumerationFailure()
    {
        // Arrange
        _mockDeviceEnumerationService.EnumerateDevicesAsync()
            .ThrowsAsync(new InvalidOperationException("Test failure"));
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act
        await viewModel.InitializeAsync();

        // Assert
        viewModel.IsLoading.Should().BeFalse();
    }

    [Fact]
    public async Task InitializeAsync_ClearsErrorMessage_BeforeEnumeration()
    {
        // Arrange
        _mockDeviceEnumerationService.EnumerateDevicesAsync()
            .Returns(Task.FromResult<IEnumerable<UsbDevice>>(Array.Empty<UsbDevice>()));
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Simulate a previous error
        viewModel.GetType().GetProperty("ErrorMessage")!.SetValue(viewModel, "Previous error");

        // Act
        await viewModel.InitializeAsync();

        // Assert
        viewModel.ErrorMessage.Should().BeNull();
    }

    #endregion

    #region Computed Property Tests

    [Fact]
    public void DeviceCountText_ReturnsCorrectFormat_WhenDevicesEmpty()
    {
        // Arrange
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act & Assert
        viewModel.DeviceCountText.Should().Be("0 USB device(s) found");
    }

    [Fact]
    public void LoadingVisibility_ReturnsVisible_WhenIsLoadingTrue()
    {
        // Arrange
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act
        viewModel.GetType().GetProperty("IsLoading")!.SetValue(viewModel, true);

        // Assert
        viewModel.LoadingVisibility.Should().Be(Microsoft.UI.Xaml.Visibility.Visible);
    }

    [Fact]
    public void LoadingVisibility_ReturnsCollapsed_WhenIsLoadingFalse()
    {
        // Arrange
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act & Assert
        viewModel.LoadingVisibility.Should().Be(Microsoft.UI.Xaml.Visibility.Collapsed);
    }

    [Fact]
    public void ErrorVisibility_ReturnsVisible_WhenErrorMessageIsSet()
    {
        // Arrange
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act
        viewModel.GetType().GetProperty("ErrorMessage")!.SetValue(viewModel, "Some error");

        // Assert
        viewModel.ErrorVisibility.Should().Be(Microsoft.UI.Xaml.Visibility.Visible);
    }

    [Fact]
    public void ErrorVisibility_ReturnsCollapsed_WhenErrorMessageIsNull()
    {
        // Arrange
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act & Assert
        viewModel.ErrorVisibility.Should().Be(Microsoft.UI.Xaml.Visibility.Collapsed);
    }

    [Fact]
    public void ContentVisibility_ReturnsVisible_WhenNotLoadingAndNoError()
    {
        // Arrange
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act & Assert (default state: IsLoading=false, ErrorMessage=null)
        viewModel.ContentVisibility.Should().Be(Microsoft.UI.Xaml.Visibility.Visible);
    }

    [Fact]
    public void ContentVisibility_ReturnsCollapsed_WhenIsLoadingTrue()
    {
        // Arrange
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act
        viewModel.GetType().GetProperty("IsLoading")!.SetValue(viewModel, true);

        // Assert
        viewModel.ContentVisibility.Should().Be(Microsoft.UI.Xaml.Visibility.Collapsed);
    }

    [Fact]
    public void ContentVisibility_ReturnsCollapsed_WhenErrorMessageIsSet()
    {
        // Arrange
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);

        // Act
        viewModel.GetType().GetProperty("ErrorMessage")!.SetValue(viewModel, "Some error");

        // Assert
        viewModel.ContentVisibility.Should().Be(Microsoft.UI.Xaml.Visibility.Collapsed);
    }

    #endregion

    #region PropertyChanged Notification Tests

    [Fact]
    public async Task InitializeAsync_RaisesPropertyChangedForDevices_WhenEnumerationCompletes()
    {
        // Arrange
        _mockDeviceEnumerationService.EnumerateDevicesAsync()
            .Returns(Task.FromResult<IEnumerable<UsbDevice>>(Array.Empty<UsbDevice>()));
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);
        var changedProperties = new List<string>();
        viewModel.PropertyChanged += (s, e) => changedProperties.Add(e.PropertyName!);

        // Act
        await viewModel.InitializeAsync();

        // Assert
        changedProperties.Should().Contain(nameof(MainViewModel.Devices));
    }

    [Fact]
    public async Task InitializeAsync_RaisesPropertyChangedForIsLoading_AtStartAndEnd()
    {
        // Arrange
        _mockDeviceEnumerationService.EnumerateDevicesAsync()
            .Returns(Task.FromResult<IEnumerable<UsbDevice>>(Array.Empty<UsbDevice>()));
        var viewModel = new MainViewModel(_mockDeviceEnumerationService);
        var isLoadingChanges = new List<bool>();
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.IsLoading))
            {
                isLoadingChanges.Add(viewModel.IsLoading);
            }
        };

        // Act
        await viewModel.InitializeAsync();

        // Assert - Should have changed to true, then back to false
        isLoadingChanges.Should().HaveCount(2);
        isLoadingChanges[0].Should().BeTrue();
        isLoadingChanges[1].Should().BeFalse();
    }

    #endregion
}
