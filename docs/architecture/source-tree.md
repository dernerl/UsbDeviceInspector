# Source Tree

```
UsbDeviceInspector/
├── .github/
│   └── workflows/
│       └── build.yml                 # GitHub Actions CI/CD pipeline
├── .vscode/
│   ├── launch.json                   # VS Code debugging configuration
│   ├── tasks.json                    # Build/test task definitions
│   └── settings.json                 # Workspace settings (C# formatting, etc.)
├── docs/
│   ├── prd.md                        # Product Requirements Document
│   ├── architecture.md               # This document
│   ├── user-personas.md              # User persona definitions
│   └── architecture-diagram.md       # Detailed architecture diagrams
├── src/
│   ├── UsbDeviceInspector/           # Main WinUI3 application project
│   │   ├── App.xaml                  # Application entry point and DI container setup
│   │   ├── App.xaml.cs               # App lifecycle, service registration
│   │   ├── Package.appxmanifest      # Windows app manifest
│   │   ├── UsbDeviceInspector.csproj # Project file with NuGet references
│   │   │
│   │   ├── Assets/                   # Application assets
│   │   │   ├── AppIcon.png           # Application icon (Windows 11 style)
│   │   │   ├── UsbIcon.png           # USB device icon for cards
│   │   │   └── SplashScreen.png      # Splash screen (optional)
│   │   │
│   │   ├── Models/                   # Data models
│   │   │   ├── UsbDevice.cs          # Core device model (ObservableObject)
│   │   │   ├── DeviceEnumerationResult.cs  # Enumeration result wrapper
│   │   │   ├── DeviceParsingError.cs # Error model
│   │   │   └── DeviceErrorType.cs    # Error type enum
│   │   │
│   │   ├── Services/                 # Business logic layer
│   │   │   ├── Interfaces/           # Service abstractions for DI
│   │   │   │   ├── IDeviceEnumerationService.cs
│   │   │   │   ├── IDeviceParsingService.cs
│   │   │   │   ├── IFormatService.cs
│   │   │   │   └── IClipboardExportService.cs
│   │   │   ├── DeviceEnumerationService.cs  # Windows API wrapper
│   │   │   ├── DeviceParsingService.cs      # Parsing logic
│   │   │   ├── FormatService.cs             # Output formatting
│   │   │   └── ClipboardExportService.cs    # Clipboard operations
│   │   │
│   │   ├── ViewModels/               # MVVM ViewModels
│   │   │   └── MainViewModel.cs      # Main window ViewModel
│   │   │
│   │   ├── Views/                    # XAML UI Views
│   │   │   └── MainWindow.xaml       # Main application window
│   │   │   └── MainWindow.xaml.cs    # Main window code-behind (minimal)
│   │   │
│   │   └── Controls/                 # Reusable XAML controls
│   │       └── DeviceCard.xaml       # Device card user control
│   │       └── DeviceCard.xaml.cs    # Device card code-behind
│   │
│   └── UsbDeviceInspector.Tests/     # Unit test project (xUnit)
│       ├── UsbDeviceInspector.Tests.csproj  # Test project file
│       │
│       ├── Services/                 # Service layer tests
│       │   ├── DeviceParsingServiceTests.cs  # Parsing logic tests
│       │   ├── FormatServiceTests.cs         # Format generation tests
│       │   └── ClipboardExportServiceTests.cs # Clipboard tests (mocked)
│       │
│       ├── ViewModels/               # ViewModel tests
│       │   └── MainViewModelTests.cs # MainViewModel unit tests
│       │
│       └── TestData/                 # Test fixtures and mock data
│           ├── MockDeviceInformation.cs  # Mock Windows API objects
│           └── SampleDevicePaths.cs      # Test device instance paths
│
├── .gitignore                        # Git ignore patterns (.NET/VS Code)
├── LICENSE                           # MIT License file
├── README.md                         # Project overview and setup guide
└── UsbDeviceInspector.sln            # Visual Studio solution file
```
