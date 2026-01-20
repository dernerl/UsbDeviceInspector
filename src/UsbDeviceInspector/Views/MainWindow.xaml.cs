using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using UsbDeviceInspector.ViewModels;
using WinRT.Interop;

namespace UsbDeviceInspector.Views;

/// <summary>
/// The main application window that displays USB device information.
/// </summary>
public sealed partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        Debug.WriteLine($"MainWindow: Window created at {DateTimeOffset.UtcNow}");

        this.InitializeComponent();
        ConfigureWindow();

        ViewModel = App.Current.Services.GetRequiredService<MainViewModel>();

        // Fire-and-forget: enumerate devices after window is rendered
        Debug.WriteLine($"MainWindow: Triggering automatic enumeration at {DateTimeOffset.UtcNow}");
        _ = ViewModel.InitializeAsync();
    }

    /// <summary>
    /// Gets the ViewModel for data binding.
    /// </summary>
    public MainViewModel ViewModel { get; }

    private void ConfigureWindow()
    {
        var hwnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        appWindow.Title = "USB Device Inspector";
        appWindow.Resize(new Windows.Graphics.SizeInt32(800, 600));
    }
}
