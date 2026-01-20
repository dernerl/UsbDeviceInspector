using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using UsbDeviceInspector.Services;
using UsbDeviceInspector.Services.Interfaces;
using UsbDeviceInspector.Views;

namespace UsbDeviceInspector;

public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
        Services = ConfigureServices();
    }

    /// <summary>
    /// Gets the current <see cref="App"/> instance.
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance for resolving dependencies.
    /// </summary>
    public IServiceProvider Services { get; }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<IDeviceEnumerationService, DeviceEnumerationService>();

        return services.BuildServiceProvider();
    }

    private Window? m_window;
}
