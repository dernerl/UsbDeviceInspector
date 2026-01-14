# USB Device Inspector - Architecture Diagram

## High-Level Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         USB DEVICE INSPECTOR                             │
│                     (WinUI3 Desktop Application)                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│                      PRESENTATION LAYER (MVVM)                           │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                           │
│  ┌─────────────────────┐              ┌──────────────────────┐          │
│  │   MainWindow.xaml   │◄─────────────│  MainViewModel       │          │
│  │  (WinUI3 View)      │  DataContext │  (MVVM ViewModel)    │          │
│  ├─────────────────────┤              ├──────────────────────┤          │
│  │ • Device Cards      │              │ Properties:          │          │
│  │ • Refresh Button    │              │ • Devices            │          │
│  │ • Copy Buttons      │              │   ObservableCollection│         │
│  │ • Empty State       │              │ • IsLoading          │          │
│  │ • Error States      │              │ • StatusMessage      │          │
│  │ • Loading Indicator │              │                      │          │
│  └─────────────────────┘              │ Commands:            │          │
│           ▲                            │ • RefreshCommand     │          │
│           │                            │ • CopyCrowdStrike    │          │
│           │ User                       │ • CopyHelpdesk       │          │
│           │ Interaction                └──────────┬───────────┘          │
│           │                                       │                      │
│           │                                       │ Dependency           │
│           │                                       │ Injection            │
└───────────┼───────────────────────────────────────┼──────────────────────┘
            │                                       │
            │                                       ▼
┌───────────┴───────────────────────────────────────────────────────────────┐
│                      BUSINESS LOGIC LAYER (Services)                       │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌───────────────────────────┐        ┌──────────────────────────┐        │
│  │ DeviceEnumerationService  │        │  DeviceParsingService    │        │
│  ├───────────────────────────┤        ├──────────────────────────┤        │
│  │ • EnumerateDevicesAsync() │───────▶│ • ParseDeviceProperties()│        │
│  │ • RefreshDevicesAsync()   │ UsbDevice│ • ParseVidPid()         │        │
│  │ • FilterUsbStorageOnly()  │        │ • ParseSerialNumber()    │        │
│  └───────────┬───────────────┘        └──────────────────────────┘        │
│              │                                                              │
│              │ Returns UsbDevice[]                                         │
│              │                                                              │
│              ▼                                                              │
│  ┌──────────────────────┐           ┌──────────────────────────┐          │
│  │   FormatService      │           │ ClipboardExportService   │          │
│  ├──────────────────────┤           ├──────────────────────────┤          │
│  │ • GenerateCrowdStrike│──────────▶│ • CopyToClipboardAsync() │          │
│  │   Format()           │  string   │ • Export success/failure │          │
│  │ • GenerateHelpdesk   │           │                          │          │
│  │   Format()           │           └────────────┬─────────────┘          │
│  └──────────────────────┘                        │                        │
│                                                   │                        │
└───────────────────────────────────────────────────┼────────────────────────┘
                                                    │
                                                    ▼
┌────────────────────────────────────────────────────────────────────────────┐
│                         DATA LAYER (Models)                                 │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐  │
│  │                         UsbDevice Model                              │  │
│  ├─────────────────────────────────────────────────────────────────────┤  │
│  │ • FriendlyName: string                                               │  │
│  │ • Manufacturer: string                                               │  │
│  │ • VendorId: string (VID)                                             │  │
│  │ • ProductId: string (PID)                                            │  │
│  │ • SerialNumber: string                                               │  │
│  │ • DeviceInstancePath: string                                         │  │
│  │ • IsValid: bool                                                      │  │
│  │ • ErrorMessage: string                                               │  │
│  │                                                                       │  │
│  │ Implements: INotifyPropertyChanged (ObservableObject)                │  │
│  └─────────────────────────────────────────────────────────────────────┘  │
│                                                                             │
│  In-Memory Collections Only (No Persistence)                               │
│                                                                             │
└────────────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────────────┐
│                    EXTERNAL DEPENDENCIES & APIs                             │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────────────────────┐  ┌───────────────────────────────┐  │
│  │ Windows.Devices.Enumeration API  │  │ Windows.ApplicationModel      │  │
│  ├──────────────────────────────────┤  │ .DataTransfer API             │  │
│  │ • DeviceInformation              │  ├───────────────────────────────┤  │
│  │   .FindAllAsync()                │  │ • DataPackage                 │  │
│  │ • Device selector queries        │  │ • SetText()                   │  │
│  │ • Standard-User compatible       │  │ • Windows Clipboard           │  │
│  └──────────────────────────────────┘  └───────────────────────────────┘  │
│                                                                             │
│  ┌──────────────────────────────────┐  ┌───────────────────────────────┐  │
│  │ CommunityToolkit.Mvvm            │  │ WinUI 3 Framework             │  │
│  ├──────────────────────────────────┤  ├───────────────────────────────┤  │
│  │ • ObservableObject               │  │ • Fluent Design System        │  │
│  │ • RelayCommand                   │  │ • AutomationProperties        │  │
│  │ • AsyncRelayCommand              │  │ • ItemsRepeater               │  │
│  └──────────────────────────────────┘  └───────────────────────────────┘  │
│                                                                             │
└────────────────────────────────────────────────────────────────────────────┘
```

## Data Flow Diagram

```
┌──────────┐
│  USER    │
│  ACTION  │
└────┬─────┘
     │
     │ 1. Clicks Refresh or App Launches
     ▼
┌─────────────────────┐
│   MainViewModel     │
│  RefreshCommand()   │
└─────────┬───────────┘
          │
          │ 2. LoadDevicesAsync()
          ▼
┌────────────────────────────┐
│ DeviceEnumerationService   │
│  EnumerateDevicesAsync()   │
└─────────┬──────────────────┘
          │
          │ 3. Query USB Devices
          ▼
┌──────────────────────────────┐
│ Windows.Devices.Enumeration  │
│   FindAllAsync()             │
└─────────┬────────────────────┘
          │
          │ 4. Returns DeviceInformation[]
          ▼
┌────────────────────────────┐
│ DeviceEnumerationService   │
│  MapToUsbDevice()          │
└─────────┬──────────────────┘
          │
          │ 5. Create UsbDevice objects
          ▼
┌───────────────────────┐
│ DeviceParsingService  │
│ ParseDeviceProperties()│
└─────────┬──────────────┘
          │
          │ 6. Extract VID/PID/Serial
          │    Set IsValid = true/false
          ▼
┌─────────────────────┐
│  UsbDevice[]        │
│  (Fully Populated)  │
└─────────┬───────────┘
          │
          │ 7. Return to ViewModel
          ▼
┌──────────────────────┐
│   MainViewModel      │
│  Devices.Add()       │
└─────────┬────────────┘
          │
          │ 8. Data Binding
          ▼
┌──────────────────────┐
│   MainWindow.xaml    │
│  Display Device Cards│
└──────────────────────┘


USER COPIES DATA:
─────────────────

┌──────────┐
│  USER    │
│  CLICKS  │
│  COPY    │
└────┬─────┘
     │
     │ 1. Click "Copy for CrowdStrike"
     ▼
┌─────────────────────────┐
│   MainViewModel         │
│  CopyCrowdStrikeCommand │
└─────────┬───────────────┘
          │
          │ 2. Pass UsbDevice
          ▼
┌────────────────────────┐
│   FormatService        │
│ GenerateCrowdStrike    │
│   Format()             │
└─────────┬──────────────┘
          │
          │ 3. Return formatted string:
          │    "VID_0781&PID_5581\4C530001234567890123"
          ▼
┌───────────────────────────┐
│  ClipboardExportService   │
│   CopyToClipboardAsync()  │
└─────────┬─────────────────┘
          │
          │ 4. Create DataPackage, SetText()
          ▼
┌──────────────────────────────┐
│ Windows Clipboard            │
│  (System-wide clipboard)     │
└──────────────────────────────┘
          │
          │ 5. Return success = true
          ▼
┌─────────────────────────┐
│   MainViewModel         │
│  StatusMessage =        │
│  "Copied to clipboard"  │
└─────────┬───────────────┘
          │
          │ 6. Update UI
          ▼
┌──────────────────────────┐
│   MainWindow.xaml        │
│  Show visual feedback    │
└──────────────────────────┘
```

## Service Responsibilities

```
┌──────────────────────────────────────────────────────────────────────┐
│                    DeviceEnumerationService                           │
├──────────────────────────────────────────────────────────────────────┤
│ Responsibilities:                                                     │
│ • Detect all connected USB storage devices                           │
│ • Filter out non-USB devices (internal SD readers)                   │
│ • Execute async to maintain UI responsiveness                        │
│ • Map DeviceInformation → UsbDevice model                            │
│ • Coordinate with DeviceParsingService for property extraction       │
│                                                                       │
│ Key Methods:                                                          │
│ • Task<IEnumerable<UsbDevice>> EnumerateDevicesAsync()               │
│ • Task<IEnumerable<UsbDevice>> RefreshDevicesAsync()                 │
│                                                                       │
│ Dependencies: DeviceParsingService                                    │
└──────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────┐
│                    DeviceParsingService                               │
├──────────────────────────────────────────────────────────────────────┤
│ Responsibilities:                                                     │
│ • Extract VID (Vendor ID) from HardwareIds array                     │
│ • Extract PID (Product ID) from HardwareIds array                    │
│ • Extract Serial Number from Device Instance Path                    │
│ • Set IsValid flag based on parsing success                          │
│ • Set ErrorMessage for parsing failures                              │
│                                                                       │
│ Key Methods:                                                          │
│ • bool ParseDeviceProperties(UsbDevice device)                       │
│ • (string VID, string PID) ParseVidPid(string[] hardwareIds)         │
│ • string ParseSerialNumber(string deviceInstancePath)                │
│                                                                       │
│ Dependencies: None (stateless utility service)                        │
└──────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────┐
│                         FormatService                                 │
├──────────────────────────────────────────────────────────────────────┤
│ Responsibilities:                                                     │
│ • Generate CrowdStrike Combined ID format                            │
│   Format: VID_xxxx&PID_xxxx\SerialNumber                             │
│ • Generate CrowdStrike Manual Entry format                           │
│ • Generate Helpdesk readable format for end users                    │
│                                                                       │
│ Key Methods:                                                          │
│ • string GenerateCrowdStrikeFormat(UsbDevice device)                 │
│ • string GenerateHelpdeskFormat(UsbDevice device)                    │
│                                                                       │
│ Dependencies: None (stateless utility service)                        │
└──────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────┐
│                   ClipboardExportService                              │
├──────────────────────────────────────────────────────────────────────┤
│ Responsibilities:                                                     │
│ • Copy formatted text to Windows clipboard                           │
│ • Handle clipboard access failures gracefully                        │
│ • Return success/failure status                                      │
│                                                                       │
│ Key Methods:                                                          │
│ • Task<bool> CopyToClipboardAsync(string content)                    │
│                                                                       │
│ Dependencies: Windows.ApplicationModel.DataTransfer API               │
└──────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────┐
│                        MainViewModel                                  │
├──────────────────────────────────────────────────────────────────────┤
│ Responsibilities:                                                     │
│ • Manage UI state (IsLoading, StatusMessage)                         │
│ • Coordinate service calls                                           │
│ • Expose device collection for data binding                          │
│ • Implement commands for user actions                                │
│ • Handle errors and update UI accordingly                            │
│                                                                       │
│ Key Properties:                                                       │
│ • ObservableCollection<UsbDevice> Devices                            │
│ • bool IsLoading                                                      │
│ • string StatusMessage                                                │
│                                                                       │
│ Key Commands:                                                         │
│ • AsyncRelayCommand RefreshCommand                                    │
│ • AsyncRelayCommand<UsbDevice> CopyCrowdStrikeCommand                │
│ • AsyncRelayCommand<UsbDevice> CopyHelpdeskCommand                   │
│                                                                       │
│ Dependencies: All Services (via constructor injection)                │
└──────────────────────────────────────────────────────────────────────┘
```

## Design Patterns Used

```
┌─────────────────────────────────────────────────────────────────┐
│                        MVVM PATTERN                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Model ────────────────▶ UsbDevice                              │
│  (Data)                  • Properties: VID, PID, Serial, etc.   │
│                          • INotifyPropertyChanged               │
│                                                                  │
│  View ─────────────────▶ MainWindow.xaml                        │
│  (UI)                    • Device Cards                         │
│                          • Buttons, Lists                       │
│                          • Data Binding                         │
│                                                                  │
│  ViewModel ────────────▶ MainViewModel                          │
│  (UI Logic)              • Commands                             │
│                          • ObservableCollection                 │
│                          • Coordinates Services                 │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                  DEPENDENCY INJECTION                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  MainViewModel Constructor:                                      │
│    public MainViewModel(                                         │
│        DeviceEnumerationService enumService,                     │
│        FormatService formatService,                              │
│        ClipboardExportService clipboardService)                  │
│    {                                                             │
│        _enumService = enumService;                               │
│        _formatService = formatService;                           │
│        _clipboardService = clipboardService;                     │
│    }                                                             │
│                                                                  │
│  Benefits:                                                       │
│  • Testability (mock services in unit tests)                    │
│  • Loose coupling between ViewModel and Services                │
│  • Single Responsibility Principle                              │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                    ASYNC/AWAIT PATTERN                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  All I/O operations are asynchronous:                            │
│                                                                  │
│  public async Task LoadDevicesAsync()                            │
│  {                                                               │
│      IsLoading = true;                                           │
│      try                                                         │
│      {                                                           │
│          var devices = await _enumService                        │
│              .EnumerateDevicesAsync();                           │
│          Devices.Clear();                                        │
│          foreach (var device in devices)                         │
│              Devices.Add(device);                                │
│      }                                                           │
│      finally                                                     │
│      {                                                           │
│          IsLoading = false;                                      │
│      }                                                           │
│  }                                                               │
│                                                                  │
│  Benefits:                                                       │
│  • UI remains responsive during device enumeration              │
│  • No "Not Responding" window states                            │
│  • Meets NFR2 requirement                                       │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                  SERVICE LAYER PATTERN                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Stateless Services:                                             │
│  • DeviceParsingService (utility/helper)                        │
│  • FormatService (utility/helper)                               │
│  • ClipboardExportService                                       │
│                                                                  │
│  Stateful Services:                                              │
│  • DeviceEnumerationService (manages enumeration state)         │
│                                                                  │
│  Benefits:                                                       │
│  • Separation of concerns                                       │
│  • Testability (unit test services independently)               │
│  • Reusability                                                  │
│  • Clear responsibilities                                       │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

## Error Handling Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                    ERROR HANDLING STRATEGY                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  1. DEVICE ENUMERATION FAILURE                                   │
│     ┌──────────────────────┐                                    │
│     │ Windows API throws   │                                    │
│     │ exception            │                                    │
│     └─────────┬────────────┘                                    │
│               │                                                  │
│               ▼                                                  │
│     ┌──────────────────────┐                                    │
│     │ Service catches      │                                    │
│     │ exception, logs,     │                                    │
│     │ returns empty list   │                                    │
│     └─────────┬────────────┘                                    │
│               │                                                  │
│               ▼                                                  │
│     ┌──────────────────────┐                                    │
│     │ ViewModel displays   │                                    │
│     │ "Device detection    │                                    │
│     │  failed" message     │                                    │
│     └──────────────────────┘                                    │
│                                                                  │
│  2. PARSING FAILURE (Missing Serial Number)                      │
│     ┌──────────────────────┐                                    │
│     │ Serial not found in  │                                    │
│     │ Device Instance Path │                                    │
│     └─────────┬────────────┘                                    │
│               │                                                  │
│               ▼                                                  │
│     ┌──────────────────────┐                                    │
│     │ Set IsValid = false  │                                    │
│     │ Set ErrorMessage     │                                    │
│     └─────────┬────────────┘                                    │
│               │                                                  │
│               ▼                                                  │
│     ┌──────────────────────┐                                    │
│     │ Device card shows    │                                    │
│     │ inline error banner  │                                    │
│     │ CrowdStrike button   │                                    │
│     │ disabled             │                                    │
│     └──────────────────────┘                                    │
│                                                                  │
│  3. CLIPBOARD OPERATION FAILURE                                  │
│     ┌──────────────────────┐                                    │
│     │ Clipboard locked by  │                                    │
│     │ another application  │                                    │
│     └─────────┬────────────┘                                    │
│               │                                                  │
│               ▼                                                  │
│     ┌──────────────────────┐                                    │
│     │ Service catches COM  │                                    │
│     │ exception, logs,     │                                    │
│     │ returns false        │                                    │
│     └─────────┬────────────┘                                    │
│               │                                                  │
│               ▼                                                  │
│     ┌──────────────────────┐                                    │
│     │ ViewModel displays   │                                    │
│     │ "Clipboard operation │                                    │
│     │  failed" message     │                                    │
│     └──────────────────────┘                                    │
│                                                                  │
│  Principle: GRACEFUL DEGRADATION                                 │
│  • Never crash the application                                  │
│  • Always show partial data when possible                       │
│  • Clear, actionable error messages                             │
│  • Errors displayed inline (not modal dialogs)                  │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

## Security & Privacy Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                   SECURITY BOUNDARIES                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌────────────────────────────────────────────────────────┐    │
│  │         APPLICATION BOUNDARY (Offline-Only)            │    │
│  │                                                         │    │
│  │  ┌──────────────────────────────────────────────┐     │    │
│  │  │  UI Layer                                     │     │    │
│  │  │  • No network calls                           │     │    │
│  │  │  • No external resources                      │     │    │
│  │  └──────────────────────────────────────────────┘     │    │
│  │                                                         │    │
│  │  ┌──────────────────────────────────────────────┐     │    │
│  │  │  Service Layer                                │     │    │
│  │  │  • No telemetry                               │     │    │
│  │  │  • No logging to files (MVP)                  │     │    │
│  │  │  • Debug.WriteLine only                       │     │    │
│  │  └──────────────────────────────────────────────┘     │    │
│  │                                                         │    │
│  │  ┌──────────────────────────────────────────────┐     │    │
│  │  │  Data Layer                                   │     │    │
│  │  │  • In-memory only                             │     │    │
│  │  │  • No persistence to disk                     │     │    │
│  │  │  • No database                                │     │    │
│  │  └──────────────────────────────────────────────┘     │    │
│  │                                                         │    │
│  └─────────────────────┬───────────────────────────────────┘    │
│                        │                                         │
│                        │ Only Interactions:                      │
│                        │                                         │
│                        ▼                                         │
│  ┌────────────────────────────────────────────────────────┐    │
│  │     WINDOWS OS APIS (Standard-User Compatible)         │    │
│  ├────────────────────────────────────────────────────────┤    │
│  │  • Windows.Devices.Enumeration (USB detection)         │    │
│  │  • Windows.ApplicationModel.DataTransfer (Clipboard)   │    │
│  │  • WinUI 3 Framework (UI rendering)                    │    │
│  │                                                         │    │
│  │  ✓ No elevation required                               │    │
│  │  ✓ No UAC prompts                                      │    │
│  │  ✓ Works on Standard-User accounts                     │    │
│  └────────────────────────────────────────────────────────┘    │
│                                                                  │
│  DATA FLOW:                                                      │
│  USB Device Info → App Memory → Clipboard → [User pastes]       │
│                                                                  │
│  NO DATA EXFILTRATION:                                           │
│  • No network transmission                                      │
│  • No file storage                                              │
│  • No telemetry/analytics                                       │
│  • No external API calls                                        │
│                                                                  │
│  ATTACK SURFACE: MINIMAL                                         │
│  • Offline-only (no remote attacks)                             │
│  • Standard-User (no privilege escalation)                      │
│  • No persistence (no data corruption)                          │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

## Deployment Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                  DEPLOYMENT MODEL (MVP)                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌────────────────────────────────────────────────────────┐    │
│  │              GitHub Repository                         │    │
│  │         (Source Code + CI/CD)                          │    │
│  └─────────────────┬──────────────────────────────────────┘    │
│                    │                                             │
│                    │ GitHub Actions                              │
│                    │ (Build, Test, Package)                      │
│                    ▼                                             │
│  ┌────────────────────────────────────────────────────────┐    │
│  │              GitHub Releases                           │    │
│  │                                                         │    │
│  │  UsbDeviceInspector-v1.0.0.zip                         │    │
│  │  ├── UsbDeviceInspector.exe                            │    │
│  │  ├── UsbDeviceInspector.dll                            │    │
│  │  ├── Microsoft.WindowsAppSDK.dll                       │    │
│  │  └── Other dependencies...                             │    │
│  │                                                         │    │
│  │  Size: ~10MB (framework-dependent)                     │    │
│  └─────────────────┬──────────────────────────────────────┘    │
│                    │                                             │
│                    │ User Downloads                              │
│                    ▼                                             │
│  ┌────────────────────────────────────────────────────────┐    │
│  │         User's Windows PC                              │    │
│  │                                                         │    │
│  │  Prerequisites:                                         │    │
│  │  • Windows 10 (1809+) or Windows 11                    │    │
│  │  • .NET 8.0 Runtime (user must install)                │    │
│  │                                                         │    │
│  │  Installation:                                          │    │
│  │  1. Extract ZIP to any folder                          │    │
│  │  2. Run UsbDeviceInspector.exe                         │    │
│  │  3. No installer, no registry, no UAC                  │    │
│  │                                                         │    │
│  │  ✓ Xcopy deployment                                    │    │
│  │  ✓ Portable (run from USB if needed)                   │    │
│  │  ✓ No admin rights required                            │    │
│  └────────────────────────────────────────────────────────┘    │
│                                                                  │
│  POST-MVP CONSIDERATION:                                         │
│  • MSIX package for Microsoft Store                             │
│  • Self-contained deployment (includes .NET runtime)            │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## Summary

**Architecture Type:** Monolithic Desktop Application

**Pattern:** MVVM (Model-View-ViewModel)

**Layers:**
1. Presentation (WinUI3 Views + ViewModels)
2. Business Logic (Stateless Services)
3. Data (In-Memory Models, No Persistence)

**Key Design Principles:**
- Separation of Concerns (MVVM + Service Layer)
- Dependency Injection (Testability)
- Async/Await (UI Responsiveness)
- Graceful Degradation (Error Handling)
- Privacy by Design (Offline-Only, Zero Telemetry)
- Standard-User Compatible (No Elevation)

**Technology Stack:**
- .NET 8.0 (LTS)
- WinUI 3 + Fluent Design System
- CommunityToolkit.Mvvm
- Windows.Devices.Enumeration API
- Windows.ApplicationModel.DataTransfer API

**Version:** 1.0 (MVP)

**Last Updated:** 2026-01-14