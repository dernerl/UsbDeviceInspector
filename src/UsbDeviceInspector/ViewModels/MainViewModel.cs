using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using UsbDeviceInspector.Models;
using UsbDeviceInspector.Services.Interfaces;

namespace UsbDeviceInspector.ViewModels;

/// <summary>
/// ViewModel for the main application window.
/// Manages device enumeration state and provides data binding for the UI.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IDeviceEnumerationService _deviceEnumerationService;

    /// <summary>
    /// The collection of enumerated USB storage devices.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DeviceCountText))]
    [NotifyPropertyChangedFor(nameof(ContentVisibility))]
    private ObservableCollection<UsbDevice> _devices = new();

    /// <summary>
    /// Indicates whether device enumeration is in progress.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LoadingVisibility))]
    [NotifyPropertyChangedFor(nameof(ContentVisibility))]
    private bool _isLoading;

    /// <summary>
    /// Contains an error message if device enumeration fails, otherwise null.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErrorVisibility))]
    [NotifyPropertyChangedFor(nameof(ContentVisibility))]
    private string? _errorMessage;

    /// <summary>
    /// Gets the visibility for the loading indicator.
    /// </summary>
    public Visibility LoadingVisibility => IsLoading ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Gets the visibility for the error display.
    /// </summary>
    public Visibility ErrorVisibility => !string.IsNullOrEmpty(ErrorMessage) ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Gets the visibility for the content display (when not loading and no error).
    /// </summary>
    public Visibility ContentVisibility => !IsLoading && string.IsNullOrEmpty(ErrorMessage) ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Gets the formatted device count text for display.
    /// </summary>
    public string DeviceCountText => $"{Devices.Count} USB device(s) found";

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="deviceEnumerationService">The device enumeration service for querying USB devices.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="deviceEnumerationService"/> is null.</exception>
    public MainViewModel(IDeviceEnumerationService deviceEnumerationService)
    {
        _deviceEnumerationService = deviceEnumerationService ?? throw new ArgumentNullException(nameof(deviceEnumerationService));
    }

    /// <summary>
    /// Initializes the ViewModel by enumerating connected USB storage devices.
    /// This method should be called after the UI window is rendered to avoid blocking display.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    public async Task InitializeAsync()
    {
        Debug.WriteLine($"MainViewModel: Starting automatic device enumeration at {DateTimeOffset.UtcNow}");
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var devices = await _deviceEnumerationService.EnumerateDevicesAsync();
            Devices = new ObservableCollection<UsbDevice>(devices);
            Debug.WriteLine($"MainViewModel: Enumeration complete. Found {Devices.Count} device(s) at {DateTimeOffset.UtcNow}");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to enumerate devices: {ex.Message}";
            Debug.WriteLine($"MainViewModel: Enumeration failed. Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}