# USB Device Inspector Frontend Architecture Document

## Change Log

| Date | Version | Description | Author |
|------|---------|-------------|--------|
| 2026-01-14 | 1.0 | Initial frontend architecture document draft | Winston (Architect) |

---

## Introduction

This document defines the **WinUI3 frontend architecture** for the USB Device Inspector desktop application. It complements the main [architecture.md](architecture.md) document by focusing exclusively on UI layer concerns: XAML patterns, ViewModels, data binding, styling, and WinUI3-specific conventions.

**Relationship to Main Architecture:**
This document references and extends the main architecture document. Core technology stack decisions in the main document are authoritative for the entire project. This document provides implementation details specific to the presentation layer.

**Framework Context:**
USB Device Inspector uses **WinUI3 (Windows App SDK)** - a native Windows UI framework, not a web framework. XAML markup is used for UI layout, C# for logic, and the MVVM pattern is mandatory for proper data binding.

**Starter Template:**
Project scaffolding based on Microsoft's official **Windows App SDK WinUI3 template** (`wasdk-winui-cs`).

---

## Frontend Tech Stack

Based on the main architecture document, here's the **WinUI3-specific technology stack** for the frontend layer:

### Technology Stack Table

| Category | Technology | Version | Purpose | Rationale |
|----------|-----------|---------|---------|-----------|
| **UI Framework** | WinUI3 (Windows App SDK) | 1.5.240802000 | Native Windows UI framework with Fluent Design | Latest stable release, provides modern Windows 11 UI controls, Fluent Design System, required for native Windows desktop development |
| **UI Language** | XAML | 2.0 | Declarative markup for UI layout and data binding | Standard markup language for WinUI3, supports compiled bindings (`x:Bind`), design-time preview, separation of UI from logic |
| **Programming Language** | C# | 12.0 | Code-behind and ViewModel logic | Latest C# with .NET 8.0, modern language features (primary constructors, collection expressions), strong typing with nullable reference types |
| **MVVM Toolkit** | CommunityToolkit.Mvvm | 8.2.2 | MVVM pattern implementation with source generators | Official Microsoft toolkit, eliminates boilerplate with `[ObservableProperty]` and `[RelayCommand]` attributes, compile-time code generation |
| **Data Binding** | x:Bind (Compiled Bindings) | Built-in | High-performance data binding between XAML and ViewModels | 2x faster than traditional `Binding`, compile-time validation prevents runtime binding errors, strongly-typed |
| **UI Controls** | WinUI 3 Controls | Built-in | Native Windows controls (ListView, Button, TextBox, etc.) | Fluent Design System compliant, automatic dark mode support, accessibility built-in, touch and pen optimized |
| **Layout System** | XAML Panels | Built-in | Grid, StackPanel, RelativePanel for responsive layouts | Declarative layout system, supports responsive design patterns, automatic layout recalculation |
| **Styling** | XAML ResourceDictionary | Built-in | Application-wide styles and theme resources | Centralized styling with StaticResource/ThemeResource, supports light/dark themes, cascading style overrides |
| **Dependency Injection** | Microsoft.Extensions.DependencyInjection | 8.0.0 | IoC container for service and ViewModel injection | Standard .NET DI container, constructor injection for ViewModels, singleton/transient lifetime management |
| **Navigation** | Custom Navigation Service | N/A (MVP) | Window management and view switching | MVP uses single MainWindow, Phase 2 may add multi-window or navigation service for settings/dialogs |
| **Testing (UI)** | Manual QA | N/A | Human validation of UI workflows | WinUI3 UI automation tooling immature, manual testing more reliable for MVP, WinAppDriver deferred to Phase 2 |
| **Testing (ViewModels)** | xUnit + NSubstitute | 2.6.6 / 5.1.0 | Unit testing ViewModels with mocked services | ViewModel logic fully testable without UI dependencies, 70%+ coverage target for command handlers |
| **Design Tools** | Visual Studio Designer / Hot Reload | Built-in | XAML visual editor and runtime UI updates | VS Code XAML preview limited, Hot Reload enables rapid UI iteration without restart |
| **Icons/Assets** | Windows App SDK Assets | Built-in | Application icon, splash screen, tile images | Standard Windows app asset formats (PNG), Fluent Design icon guidelines |

**Key Technology Decisions:**
- **WinUI3 over WPF:** WinUI3 is the modern Windows UI framework with active development, WPF is in maintenance mode
- **x:Bind over traditional Binding:** Compiled bindings provide compile-time safety and 2x performance improvement - mandatory for this project per coding standards
- **CommunityToolkit.Mvvm over manual MVVM:** Source generators eliminate 70% of boilerplate code, official Microsoft support ensures compatibility
- **No separate routing library:** Single-window MVP doesn't need complex navigation, avoiding over-engineering
- **Manual UI testing for MVP:** WinUI3 UI automation is unstable, manual QA provides better ROI until ecosystem matures

---

## Project Structure

Here's the **exact frontend directory structure** for the WinUI3 application, following Windows App SDK conventions:

```
src/UsbDeviceInspector/
├── App.xaml                          # Application-level XAML (styles, resources, theme)
├── App.xaml.cs                       # Application entry point, DI container setup
├── Package.appxmanifest              # Windows app manifest (capabilities, assets)
├── UsbDeviceInspector.csproj         # Project file with WinUI3 NuGet packages
│
├── Assets/                           # Application assets and icons
│   ├── AppIcon.png                   # Application icon (44x44, 150x150, 310x310)
│   ├── UsbIcon.png                   # USB device icon for cards
│   ├── SplashScreen.png              # Splash screen (620x300)
│   ├── Square44x44Logo.png           # Tile icon (Start menu)
│   ├── Square150x150Logo.png         # Medium tile
│   └── Wide310x150Logo.png           # Wide tile
│
├── Models/                           # Data models (domain entities)
│   ├── UsbDevice.cs                  # Core device model (ObservableObject)
│   ├── DeviceEnumerationResult.cs    # Enumeration result wrapper
│   ├── DeviceParsingError.cs         # Error model for UI display
│   └── DeviceErrorType.cs            # Error type enum
│
├── ViewModels/                       # MVVM ViewModels (presentation logic)
│   ├── MainViewModel.cs              # Main window ViewModel
│   └── ViewModelBase.cs              # Base class (optional, if shared logic exists)
│
├── Views/                            # XAML Views (UI pages/windows)
│   ├── MainWindow.xaml               # Main application window (UI markup)
│   └── MainWindow.xaml.cs            # Code-behind (minimal, event wiring only)
│
├── Controls/                         # Reusable XAML user controls
│   ├── DeviceCard.xaml               # Device card user control (UI markup)
│   ├── DeviceCard.xaml.cs            # Code-behind for device card
│   └── EmptyStateView.xaml           # Empty state UI (no devices found)
│
├── Styles/                           # XAML styling resources
│   ├── AppStyles.xaml                # Application-wide control styles
│   ├── ColorPalette.xaml             # Custom color definitions (extends Fluent)
│   └── Typography.xaml               # Text styles and font definitions
│
├── Converters/                       # XAML value converters
│   ├── BoolToVisibilityConverter.cs  # Boolean to Visibility conversion
│   ├── NullToVisibilityConverter.cs  # Null check for visibility
│   └── ErrorTypeToMessageConverter.cs # Error enum to user message
│
├── Services/                         # Business logic services (injected into ViewModels)
│   ├── Interfaces/                   # Service abstractions
│   │   ├── IDeviceEnumerationService.cs
│   │   ├── IDeviceParsingService.cs
│   │   ├── IFormatService.cs
│   │   └── IClipboardExportService.cs
│   ├── DeviceEnumerationService.cs   # Windows API wrapper
│   ├── DeviceParsingService.cs       # Device parsing logic
│   ├── FormatService.cs              # Output formatting
│   └── ClipboardExportService.cs     # Clipboard operations
│
└── Helpers/                          # Utility classes
    ├── RelayCommandAsync.cs          # Custom async command (if needed)
    └── DebugLogger.cs                # Debug logging helper (MVP only)
```

**Key Organizational Principles:**
- **Views + ViewModels Separation:** Clear MVVM separation. Views contain only XAML + minimal code-behind, ViewModels contain all presentation logic
- **Controls Directory:** Reusable UI components live in dedicated folder for component reuse
- **Styles Directory:** Centralized styling keeps XAML files clean
- **Converters Directory:** XAML value converters enable complex data transformations in bindings
- **Services at Root Level:** Services shared between frontend and backend in monolithic architecture

---

## Component Standards

### Component Template

**File: `Controls/ExampleControl.xaml`**

```xml
<UserControl
    x:Class="UsbDeviceInspector.Controls.ExampleControl"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d">

    <UserControl.Resources>
        <!-- Component-specific resources (styles, converters) -->
        <SolidColorBrush x:Key="ExampleBrush" Color="{ThemeResource SystemAccentColor}" />
    </UserControl.Resources>

    <Grid Padding="16" CornerRadius="8" Background="{ThemeResource CardBackgroundFillColorDefaultBrush}">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>

        <!-- Header -->
        <TextBlock
            Grid.Row="0"
            Text="{x:Bind Title, Mode=OneWay}"
            Style="{StaticResource SubtitleTextBlockStyle}" />

        <!-- Content -->
        <StackPanel Grid.Row="1" Spacing="8" Margin="0,12,0,0">
            <TextBlock
                Text="{x:Bind Description, Mode=OneWay}"
                TextWrapping="Wrap"
                Foreground="{ThemeResource TextFillColorSecondaryBrush}" />
        </StackPanel>
    </Grid>
</UserControl>
```

**File: `Controls/ExampleControl.xaml.cs`**

```csharp
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace UsbDeviceInspector.Controls;

/// <summary>
/// Example reusable user control following WinUI3 MVVM patterns.
/// </summary>
public sealed partial class ExampleControl : UserControl
{
    public ExampleControl()
    {
        this.InitializeComponent();
    }

    // Dependency Property for Title
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(ExampleControl),
            new PropertyMetadata(string.Empty));

    // Dependency Property for Description
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(ExampleControl),
            new PropertyMetadata(string.Empty));
}
```

### Naming Conventions

#### File Naming
- **XAML Views:** PascalCase ending in `.xaml` (e.g., `MainWindow.xaml`, `SettingsPage.xaml`)
- **Code-Behind:** Matching XAML name with `.xaml.cs` extension (e.g., `MainWindow.xaml.cs`)
- **ViewModels:** PascalCase ending in `ViewModel.cs` (e.g., `MainViewModel.cs`, `DeviceCardViewModel.cs`)
- **User Controls:** PascalCase descriptive names (e.g., `DeviceCard.xaml`, `EmptyStateView.xaml`)
- **Converters:** PascalCase ending in `Converter.cs` (e.g., `BoolToVisibilityConverter.cs`)
- **Services:** PascalCase ending in `Service.cs` (e.g., `DeviceEnumerationService.cs`)

#### Component Naming
- **UserControl Class Names:** Match filename exactly (e.g., `DeviceCard.xaml` → `class DeviceCard`)
- **ViewModels:** Match corresponding View with `ViewModel` suffix (e.g., `MainWindow` → `MainViewModel`)
- **XAML x:Name:** camelCase for control instances (e.g., `x:Name="deviceListView"`, `x:Name="copyButton"`)

#### Property Naming
- **Dependency Properties:** PascalCase with `Property` suffix for the field (e.g., `TitleProperty`, `IsLoadingProperty`)
- **CLR Property Wrappers:** PascalCase matching the dependency property name (e.g., `Title`, `IsLoading`)
- **ViewModel Properties (ObservableProperty):** PascalCase for public properties (e.g., `DeviceList`, `SelectedDevice`, `IsRefreshing`)
- **Private Fields:** `_camelCase` with underscore prefix (e.g., `_deviceService`, `_logger`)

#### Command Naming
- **RelayCommand Properties:** PascalCase ending in `Command` (e.g., `RefreshDevicesCommand`, `CopyForCrowdStrikeCommand`)
- **Command Handler Methods:** Matching command name without `Command` suffix (e.g., `RefreshDevices()`, `CopyForCrowdStrike()`)

#### Resource Naming
- **Styles:** PascalCase descriptive names (e.g., `PrimaryButtonStyle`, `DeviceCardContainerStyle`)
- **Brushes:** PascalCase ending in `Brush` (e.g., `AccentBorderBrush`, `ErrorBackgroundBrush`)
- **Templates:** PascalCase ending in `Template` (e.g., `DeviceCardTemplate`, `EmptyStateTemplate`)

#### XAML Namespace Prefixes
- **Default (WinUI3):** No prefix (e.g., `<Button>`, `<Grid>`)
- **XAML namespace:** `x:` prefix (e.g., `x:Bind`, `x:Name`, `x:Class`)
- **Local controls:** `controls:` prefix (e.g., `<controls:DeviceCard>`)
- **Local converters:** `converters:` prefix (e.g., `<converters:BoolToVisibilityConverter>`)
- **ViewModels:** `viewmodels:` prefix for design-time data (e.g., `d:DataContext="{d:DesignInstance Type=viewmodels:MainViewModel}"`)

---

## State Management

### Store Structure

WinUI3 uses **ViewModel-based state management** with `CommunityToolkit.Mvvm` rather than centralized stores like Redux/Vuex. Each ViewModel manages its own observable state.

```
src/UsbDeviceInspector/
│
├── ViewModels/                       # ViewModels contain observable state
│   ├── MainViewModel.cs              # Main window state (device list, loading, errors)
│   └── ViewModelBase.cs              # Optional base class for shared logic
│
├── Models/                           # Data models (can be observable)
│   ├── UsbDevice.cs                  # Observable device model
│   └── DeviceEnumerationResult.cs    # Result wrapper
│
├── Services/                         # State persistence (if needed)
│   └── Interfaces/
│       └── IStateService.cs          # Optional: save/restore app state (Phase 2)
│
└── App.xaml.cs                       # Application-level state (DI container)
```

**Key Concepts:**
- **No centralized store:** Each ViewModel is its own "store" with observable properties
- **Services for shared state:** If multiple ViewModels need same data, inject shared service
- **Application-level state:** App.xaml.cs holds DI container (singleton services act as shared state)
- **No state persistence in MVP:** All state is transient (cleared on app exit)

### State Management Template

**File: `ViewModels/MainViewModel.cs`**

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UsbDeviceInspector.Models;
using UsbDeviceInspector.Services.Interfaces;

namespace UsbDeviceInspector.ViewModels;

/// <summary>
/// Main window ViewModel managing device list state and user commands.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IDeviceEnumerationService _deviceEnumerationService;
    private readonly IDeviceParsingService _deviceParsingService;
    private readonly IFormatService _formatService;
    private readonly IClipboardExportService _clipboardExportService;

    // Constructor injection for services
    public MainViewModel(
        IDeviceEnumerationService deviceEnumerationService,
        IDeviceParsingService deviceParsingService,
        IFormatService formatService,
        IClipboardExportService clipboardExportService)
    {
        _deviceEnumerationService = deviceEnumerationService;
        _deviceParsingService = deviceParsingService;
        _formatService = formatService;
        _clipboardExportService = clipboardExportService;

        // Initialize on construction
        _ = LoadDevicesAsync();
    }

    // Observable properties (auto-generated by source generators)
    [ObservableProperty]
    private ObservableCollection<UsbDevice> _devices = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _hasDevices;

    [ObservableProperty]
    private UsbDevice? _selectedDevice;

    // Computed property (derives from other state)
    public bool ShowEmptyState => !IsLoading && !HasDevices;

    // Relay commands (auto-generated by source generators)
    [RelayCommand]
    private async Task LoadDevicesAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var deviceInfoList = await _deviceEnumerationService.EnumerateUsbStorageDevicesAsync();
            var result = _deviceParsingService.ParseDevices(deviceInfoList);

            Devices.Clear();
            foreach (var device in result.Devices)
            {
                Devices.Add(device);
            }

            HasDevices = Devices.Count > 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to enumerate devices: {ex.Message}";
            Debug.WriteLine($"[ERROR] Device enumeration failed: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshDevicesAsync()
    {
        IsRefreshing = true;
        await LoadDevicesAsync();
        IsRefreshing = false;
    }

    [RelayCommand(CanExecute = nameof(CanCopyForCrowdStrike))]
    private async Task CopyForCrowdStrikeAsync(UsbDevice device)
    {
        try
        {
            var formattedText = _formatService.GenerateCrowdStrikeCombinedId(device);
            var success = await _clipboardExportService.TrySetClipboardDataAsync(formattedText);

            if (!success)
            {
                ErrorMessage = "Failed to copy to clipboard. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Clipboard error: {ex.Message}";
            Debug.WriteLine($"[ERROR] Clipboard operation failed: {ex}");
        }
    }

    private bool CanCopyForCrowdStrike(UsbDevice? device)
    {
        return device?.HasSerialNumber == true;
    }

    // Manual property change notification for computed properties
    partial void OnIsLoadingChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowEmptyState));
    }

    partial void OnHasDevicesChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowEmptyState));
    }
}
```

---

## API Integration

### Service Template

WinUI3 applications integrate with **Windows Runtime APIs** (not REST APIs). Here's the pattern for wrapping Windows APIs in testable service classes:

**File: `Services/DeviceEnumerationService.cs`**

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using UsbDeviceInspector.Services.Interfaces;

namespace UsbDeviceInspector.Services;

/// <summary>
/// Service wrapping Windows.Devices.Enumeration API for USB device discovery.
/// </summary>
public class DeviceEnumerationService : IDeviceEnumerationService
{
    private const int MaxRetryAttempts = 3;
    private const int RetryDelayMs = 500;

    /// <summary>
    /// Enumerates USB storage devices using Windows Runtime API.
    /// </summary>
    /// <returns>List of DeviceInformation objects for connected USB devices.</returns>
    /// <exception cref="DeviceEnumerationException">Thrown when enumeration fails after retries.</exception>
    public async Task<IReadOnlyList<DeviceInformation>> EnumerateUsbStorageDevicesAsync()
    {
        var attempts = 0;
        Exception? lastException = null;

        while (attempts < MaxRetryAttempts)
        {
            try
            {
                attempts++;
                Debug.WriteLine($"[DeviceEnumeration] Attempt {attempts}/{MaxRetryAttempts}");

                // Build Advanced Query Syntax (AQS) selector
                var selector = GetDeviceSelector();

                // Query Windows API for devices (async operation)
                var deviceInfoCollection = await DeviceInformation.FindAllAsync(selector)
                    .AsTask()
                    .ConfigureAwait(false);

                Debug.WriteLine($"[DeviceEnumeration] Found {deviceInfoCollection.Count} devices");

                return deviceInfoCollection;
            }
            catch (UnauthorizedAccessException ex)
            {
                lastException = ex;
                Debug.WriteLine($"[DeviceEnumeration] Access denied: {ex.Message}");

                // Don't retry - permission issue won't resolve
                throw new DeviceEnumerationException(
                    "Windows denied access to device information. Ensure application is not blocked by security software.",
                    ex);
            }
            catch (Exception ex)
            {
                lastException = ex;
                Debug.WriteLine($"[DeviceEnumeration] Attempt {attempts} failed: {ex.Message}");

                if (attempts < MaxRetryAttempts)
                {
                    // Exponential backoff
                    var delay = RetryDelayMs * attempts;
                    await Task.Delay(delay).ConfigureAwait(false);
                }
            }
        }

        // All retries exhausted
        throw new DeviceEnumerationException(
            $"Device enumeration failed after {MaxRetryAttempts} attempts. System may be under heavy load.",
            lastException!);
    }

    /// <summary>
    /// Builds AQS filter to exclude internal SD card readers and non-USB devices.
    /// </summary>
    private string GetDeviceSelector()
    {
        // Query for PortableStorageDevice class (includes USB storage)
        var baseSelector = DeviceInformation.GetAqsFilterFromDeviceClass(DeviceClass.PortableStorageDevice);

        // Additional filtering done in parsing layer (Device Instance Path validation)
        return baseSelector;
    }
}

/// <summary>
/// Custom exception for device enumeration failures.
/// </summary>
public class DeviceEnumerationException : Exception
{
    public DeviceEnumerationException(string message) : base(message) { }
    public DeviceEnumerationException(string message, Exception innerException)
        : base(message, innerException) { }
}
```

### API Client Configuration

**File: `App.xaml.cs` (Dependency Injection Setup)**

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using UsbDeviceInspector.Services;
using UsbDeviceInspector.Services.Interfaces;
using UsbDeviceInspector.ViewModels;
using UsbDeviceInspector.Views;

namespace UsbDeviceInspector;

/// <summary>
/// Application entry point and DI container configuration.
/// </summary>
public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        this.InitializeComponent();

        // Configure dependency injection container
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Register all services and ViewModels with DI container.
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        // Register services as singletons (stateless, safe to reuse)
        services.AddSingleton<IDeviceEnumerationService, DeviceEnumerationService>();
        services.AddSingleton<IDeviceParsingService, DeviceParsingService>();
        services.AddSingleton<IFormatService, FormatService>();
        services.AddSingleton<IClipboardExportService, ClipboardExportService>();

        // Register ViewModels as transient (new instance per request)
        services.AddTransient<MainViewModel>();

        // Register Views as transient
        services.AddTransient<MainWindow>();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Resolve MainWindow from DI container (automatically injects dependencies)
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Activate();
    }
}
```

**File: `Views/MainWindow.xaml.cs` (ViewModel Injection)**

```csharp
using Microsoft.UI.Xaml;
using UsbDeviceInspector.ViewModels;

namespace UsbDeviceInspector.Views;

/// <summary>
/// Main application window with injected ViewModel.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }

    // Constructor injection - DI container provides ViewModel
    public MainWindow(MainViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;

        // Set DataContext for x:Bind compilation
        this.DataContext = ViewModel;
    }
}
```

---

## Routing

### Route Configuration

**Note:** This application uses a **single-window architecture** with no routing system for MVP. Multi-window navigation and settings dialogs are deferred to Phase 2.

**Current MVP Navigation Pattern:**

```csharp
// File: App.xaml.cs
protected override void OnLaunched(LaunchActivatedEventArgs args)
{
    // Single MainWindow instance - no routing needed
    var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
    mainWindow.Activate();
}
```

**Phase 2 Navigation Service Pattern (Future):**

If Phase 2 adds settings dialogs or multi-window support, implement a navigation service:

```csharp
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;

namespace UsbDeviceInspector.Services;

/// <summary>
/// Navigation service for multi-window management (Phase 2).
/// </summary>
public interface INavigationService
{
    void Navigate<TWindow>() where TWindow : Window;
    void Close(Window window);
    Task<bool> ShowDialogAsync<TDialog>(object? parameter = null) where TDialog : ContentDialog;
}

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Window> _openWindows = new();

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Navigate<TWindow>() where TWindow : Window
    {
        var window = _serviceProvider.GetRequiredService<TWindow>();
        _openWindows.Add(window);
        window.Closed += (s, e) => _openWindows.Remove(window);
        window.Activate();
    }

    public void Close(Window window)
    {
        _openWindows.Remove(window);
        window.Close();
    }

    public async Task<bool> ShowDialogAsync<TDialog>(object? parameter = null) where TDialog : ContentDialog
    {
        var dialog = _serviceProvider.GetRequiredService<TDialog>();
        var currentWindow = _openWindows.LastOrDefault();
        if (currentWindow?.Content is FrameworkElement root)
        {
            dialog.XamlRoot = root.XamlRoot;
        }
        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }
}
```

---

## Styling Guidelines

### Styling Approach

WinUI3 uses **XAML ResourceDictionary-based styling** with support for light/dark themes.

**File: `Styles/AppStyles.xaml`**

```xml
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- Card Container Style -->
    <Style x:Key="CardContainerStyle" TargetType="Grid">
        <Setter Property="Background" Value="{ThemeResource CardBackgroundFillColorDefaultBrush}" />
        <Setter Property="BorderBrush" Value="{ThemeResource CardStrokeColorDefaultBrush}" />
        <Setter Property="BorderThickness" Value="1" />
        <Setter Property="CornerRadius" Value="8" />
        <Setter Property="Padding" Value="16" />
        <Setter Property="Margin" Value="0,0,0,12" />
    </Style>

    <!-- Primary Action Button Style -->
    <Style x:Key="PrimaryButtonStyle" TargetType="Button" BasedOn="{StaticResource AccentButtonStyle}">
        <Setter Property="MinWidth" Value="120" />
        <Setter Property="Height" Value="36" />
        <Setter Property="CornerRadius" Value="4" />
        <Setter Property="FontWeight" Value="SemiBold" />
    </Style>

    <!-- Device Title Text Style -->
    <Style x:Key="DeviceTitleTextStyle" TargetType="TextBlock" BasedOn="{StaticResource SubtitleTextBlockStyle}">
        <Setter Property="FontWeight" Value="SemiBold" />
        <Setter Property="Foreground" Value="{ThemeResource TextFillColorPrimaryBrush}" />
        <Setter Property="TextTrimming" Value="CharacterEllipsis" />
    </Style>

    <!-- Error Message Text Style -->
    <Style x:Key="ErrorTextStyle" TargetType="TextBlock" BasedOn="{StaticResource CaptionTextBlockStyle}">
        <Setter Property="Foreground" Value="{ThemeResource SystemFillColorCriticalBrush}" />
        <Setter Property="TextWrapping" Value="Wrap" />
    </Style>

</ResourceDictionary>
```

### Global Theme Variables

**File: `Styles/ColorPalette.xaml`**

```xml
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- Brand Colors -->
    <Color x:Key="BrandPrimaryColor">#0078D4</Color>
    <SolidColorBrush x:Key="BrandPrimaryBrush" Color="{StaticResource BrandPrimaryColor}" />

    <!-- Semantic Colors (Light/Dark Theme) -->
    <ResourceDictionary.ThemeDictionaries>
        <ResourceDictionary x:Key="Light">
            <SolidColorBrush x:Key="SuccessBrush" Color="#107C10" />
            <SolidColorBrush x:Key="WarningBrush" Color="#F7630C" />
            <SolidColorBrush x:Key="ErrorBrush" Color="#D13438" />
        </ResourceDictionary>

        <ResourceDictionary x:Key="Dark">
            <SolidColorBrush x:Key="SuccessBrush" Color="#6CCB5F" />
            <SolidColorBrush x:Key="WarningBrush" Color="#FCE100" />
            <SolidColorBrush x:Key="ErrorBrush" Color="#FF99A4" />
        </ResourceDictionary>
    </ResourceDictionary.ThemeDictionaries>

    <!-- Spacing Scale -->
    <Thickness x:Key="SpacingXSmall">4</Thickness>
    <Thickness x:Key="SpacingSmall">8</Thickness>
    <Thickness x:Key="SpacingMedium">16</Thickness>
    <Thickness x:Key="SpacingLarge">24</Thickness>

    <!-- Border Radius Values -->
    <CornerRadius x:Key="CornerRadiusSmall">4</CornerRadius>
    <CornerRadius x:Key="CornerRadiusMedium">8</CornerRadius>
    <CornerRadius x:Key="CornerRadiusLarge">12</CornerRadius>

</ResourceDictionary>
```

---

## Testing Requirements

### Component Test Template

**File: `UsbDeviceInspector.Tests/ViewModels/MainViewModelTests.cs`**

```csharp
using FluentAssertions;
using NSubstitute;
using UsbDeviceInspector.Models;
using UsbDeviceInspector.Services.Interfaces;
using UsbDeviceInspector.ViewModels;
using Xunit;

namespace UsbDeviceInspector.Tests.ViewModels;

public class MainViewModelTests
{
    private readonly IDeviceEnumerationService _mockEnumerationService;
    private readonly IDeviceParsingService _mockParsingService;
    private readonly IFormatService _mockFormatService;
    private readonly IClipboardExportService _mockClipboardService;
    private readonly MainViewModel _viewModel;

    public MainViewModelTests()
    {
        // Arrange: Create mocks
        _mockEnumerationService = Substitute.For<IDeviceEnumerationService>();
        _mockParsingService = Substitute.For<IDeviceParsingService>();
        _mockFormatService = Substitute.For<IFormatService>();
        _mockClipboardService = Substitute.For<IClipboardExportService>();

        _viewModel = new MainViewModel(
            _mockEnumerationService,
            _mockParsingService,
            _mockFormatService,
            _mockClipboardService);
    }

    [Fact]
    public async Task LoadDevicesAsync_WithValidDevices_PopulatesDeviceCollection()
    {
        // Arrange
        var mockDevices = new List<UsbDevice>
        {
            new UsbDevice { FriendlyName = "SanDisk USB", VendorId = "0781" }
        };
        var result = new DeviceEnumerationResult { Devices = mockDevices };

        _mockEnumerationService.EnumerateUsbStorageDevicesAsync()
            .Returns(Task.FromResult<IReadOnlyList<DeviceInformation>>(new List<DeviceInformation>()));
        _mockParsingService.ParseDevices(Arg.Any<IReadOnlyList<DeviceInformation>>())
            .Returns(result);

        // Act
        await _viewModel.LoadDevicesCommand.ExecuteAsync(null);

        // Assert
        _viewModel.Devices.Should().HaveCount(1);
        _viewModel.HasDevices.Should().BeTrue();
    }

    [Fact]
    public void CopyForCrowdStrikeCommand_WithDeviceMissingSerial_CannotExecute()
    {
        // Arrange
        var device = new UsbDevice { HasSerialNumber = false };

        // Act
        var canExecute = _viewModel.CopyForCrowdStrikeCommand.CanExecute(device);

        // Assert
        canExecute.Should().BeFalse();
    }
}
```

### Testing Best Practices

1. **Unit Tests**: Test ViewModels and services in isolation with mocked dependencies
2. **Integration Tests**: Test service layer against real Windows APIs
3. **E2E Tests**: Manual QA testing of full user workflows
4. **Coverage Goals**: 80% service layer, 70% ViewModels
5. **Test Structure**: AAA (Arrange-Act-Assert) pattern
6. **Mock External Dependencies**: Mock Windows APIs using NSubstitute

---

## Environment Configuration

### Environment Variables

WinUI3 desktop applications **do not use environment variables** like web applications. Configuration handled through:

**File: `Package.appxmanifest`**

```xml
<?xml version="1.0" encoding="utf-8"?>
<Package xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10">
  <Identity Name="UsbDeviceInspector" Version="1.0.0.0" />
  <Properties>
    <DisplayName>USB Device Inspector</DisplayName>
  </Properties>
  <Dependencies>
    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.19041.0" />
  </Dependencies>
</Package>
```

**Debug vs Release Configuration:**

```xml
<!-- UsbDeviceInspector.csproj -->
<PropertyGroup Condition="'$(Configuration)'=='Debug'">
  <DefineConstants>DEBUG;TRACE</DefineConstants>
  <Optimize>false</Optimize>
</PropertyGroup>

<PropertyGroup Condition="'$(Configuration)'=='Release'">
  <DefineConstants>RELEASE;TRACE</DefineConstants>
  <Optimize>true</Optimize>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

---

## Frontend Developer Standards

### Critical Coding Rules

**Essential rules preventing common WinUI3 mistakes:**

#### 1. XAML Data Binding Rules
- ✅ **ALWAYS use `x:Bind` with explicit Mode**
  ```xml
  <TextBlock Text="{x:Bind ViewModel.DeviceName, Mode=OneWay}" />
  ```
- ❌ **NEVER use `{Binding}` markup** - Use compiled bindings

#### 2. Async/Await Rules
- ✅ **ALWAYS use `async Task` for async commands**
- ✅ **ALWAYS use `.ConfigureAwait(false)` in service layer**
- ❌ **NEVER use `.Result` or `.Wait()`** - Causes deadlocks

#### 3. ObservableProperty Rules
- ✅ **ALWAYS use `[ObservableProperty]` on private fields**
  ```csharp
  [ObservableProperty]
  private string _deviceName;
  ```
- ✅ **ALWAYS use `partial` class modifier**

#### 4. Dependency Injection Rules
- ✅ **ALWAYS inject services via constructor**
- ❌ **NEVER use `new` keyword in ViewModels**

#### 5. Resource and Styling Rules
- ✅ **ALWAYS use `{ThemeResource}` for colors**
- ❌ **NEVER hardcode hex colors in XAML**

#### 6. Privacy and Logging Rules
- ❌ **NEVER log Device Instance Paths in production**
- ✅ **ALWAYS use `[Conditional("DEBUG")]` for debug logging**

### Quick Reference

**Common Commands:**
```bash
# Run application
dotnet run --project src/UsbDeviceInspector/UsbDeviceInspector.csproj

# Build solution
dotnet build

# Run tests
dotnet test

# Build Release
dotnet build -c Release
```

**Key Import Patterns:**
```csharp
// XAML
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// MVVM
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

// Windows APIs
using Windows.Devices.Enumeration;
using Windows.ApplicationModel.DataTransfer;

// DI
using Microsoft.Extensions.DependencyInjection;
```

**File Naming Conventions:**

| Type | Convention | Example |
|------|-----------|---------|
| XAML Views | PascalCase.xaml | MainWindow.xaml |
| ViewModels | PascalCaseViewModel.cs | MainViewModel.cs |
| Services | PascalCaseService.cs | DeviceEnumerationService.cs |
| User Controls | PascalCase.xaml | DeviceCard.xaml |

---

## Conclusion

This frontend architecture document provides WinUI3-specific implementation guidance for the USB Device Inspector desktop application. Key principles:

- **MVVM Pattern:** Strict separation of Views (XAML) and ViewModels (C#)
- **x:Bind Compiled Bindings:** Performance and compile-time safety
- **CommunityToolkit.Mvvm:** Source generators eliminate boilerplate
- **Dependency Injection:** Testable, maintainable code
- **ThemeResource Styling:** Automatic light/dark mode support
- **Windows Runtime APIs:** Native Windows integration

For backend architecture, data models, and deployment details, refer to the main [architecture.md](architecture.md) document.