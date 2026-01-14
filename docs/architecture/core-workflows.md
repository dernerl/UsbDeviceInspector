# Core Workflows

## Workflow 1: Application Launch and Initial Device Enumeration

```mermaid
sequenceDiagram
    actor User
    participant App as App.xaml.cs
    participant DI as DI Container
    participant MainWindow as MainWindow
    participant VM as MainViewModel
    participant EnumSvc as DeviceEnumerationService
    participant ParseSvc as DeviceParsingService
    participant WinAPI as Windows.Devices.Enumeration

    User->>App: Launch Application
    App->>DI: Configure Services
    DI->>DI: Register Singleton Services
    App->>MainWindow: Create & Show Window
    MainWindow->>DI: Resolve MainViewModel
    DI->>VM: Create with injected services
    VM->>VM: Initialize()

    Note over VM: Set IsLoading = true
    VM->>EnumSvc: EnumerateUsbStorageDevicesAsync()
    EnumSvc->>WinAPI: FindAllAsync(PortableStorageDevice)

    Note over WinAPI: Query USB devices (async)
    WinAPI-->>EnumSvc: IReadOnlyList<DeviceInformation>

    EnumSvc-->>VM: Return device info list
    VM->>ParseSvc: ParseDevices(deviceInfoList)

    loop For each DeviceInformation
        ParseSvc->>ParseSvc: Extract VID/PID from HardwareIds
        ParseSvc->>ParseSvc: Extract Serial from DeviceInstancePath
        alt Parsing successful
            ParseSvc->>ParseSvc: Create UsbDevice model
        else Parsing failed
            ParseSvc->>ParseSvc: Create DeviceParsingError
        end
    end

    ParseSvc-->>VM: DeviceEnumerationResult

    Note over VM: Set IsLoading = false
    VM->>VM: Update ObservableCollection<UsbDevice>
    VM->>MainWindow: OnPropertyChanged (Devices)
    MainWindow->>User: Display device cards

    alt No devices found
        MainWindow->>User: Show empty state message
    end

    alt Parsing errors occurred
        MainWindow->>User: Show error cards inline
    end
```

## Workflow 2: Copy for CrowdStrike Export

```mermaid
sequenceDiagram
    actor User
    participant Card as DeviceCard UI
    participant VM as MainViewModel
    participant FmtSvc as FormatService
    participant ClipSvc as ClipboardExportService
    participant WinClip as Windows Clipboard API

    User->>Card: Click "Copy for CrowdStrike"
    Card->>VM: CopyForCrowdStrikeCommand.Execute(device)

    Note over VM: Validate device has serial number
    alt Device missing serial number
        VM->>Card: Show error tooltip
        Card->>User: Display "Serial number required" message
    else Device valid
        VM->>FmtSvc: GenerateCrowdStrikeCombinedId(device)
        FmtSvc->>FmtSvc: Format: VID_{vid}&PID_{pid}\{serial}
        FmtSvc-->>VM: "VID_0781&PID_5581\4C530001..."

        VM->>FmtSvc: GenerateCrowdStrikeManualEntry(device)
        FmtSvc->>FmtSvc: Format multi-line with VID/PID/Serial
        FmtSvc-->>VM: Multi-line text block

        VM->>ClipSvc: CopyToClipboardAsync(combinedText)
        ClipSvc->>WinClip: Create DataPackage
        ClipSvc->>WinClip: SetText(combinedText)
        ClipSvc->>WinClip: Clipboard.SetContent(dataPackage)

        alt Clipboard operation successful
            WinClip-->>ClipSvc: Success
            ClipSvc-->>VM: return true
            VM->>VM: Set IsCopySuccessful = true (temp)
            VM->>Card: Trigger success animation
            Card->>User: Show checkmark icon (2 seconds)
        else Clipboard locked by another app
            WinClip-->>ClipSvc: Exception
            ClipSvc-->>VM: return false
            VM->>Card: Show error notification
            Card->>User: Display "Clipboard unavailable" message
        end
    end
```

## Workflow 3: Manual Refresh with New Device Detection

```mermaid
sequenceDiagram
    actor User
    participant MainWindow as MainWindow
    participant VM as MainViewModel
    participant EnumSvc as DeviceEnumerationService
    participant ParseSvc as DeviceParsingService
    participant WinAPI as Windows.Devices.Enumeration

    Note over User: User plugs in new USB device

    User->>MainWindow: Click "Refresh" button
    MainWindow->>VM: RefreshDevicesCommand.Execute()

    Note over VM: Set IsRefreshing = true
    VM->>VM: Clear existing Devices collection

    VM->>EnumSvc: EnumerateUsbStorageDevicesAsync()
    EnumSvc->>WinAPI: FindAllAsync(PortableStorageDevice)

    Note over WinAPI: Re-enumerate all USB devices
    WinAPI-->>EnumSvc: Updated device list (includes new device)

    EnumSvc-->>VM: Device info list
    VM->>ParseSvc: ParseDevices(deviceInfoList)
    ParseSvc-->>VM: DeviceEnumerationResult

    Note over VM: Set IsRefreshing = false
    VM->>VM: Update ObservableCollection

    alt New devices detected
        VM->>MainWindow: Scroll to first new device
        MainWindow->>User: Highlight newly detected devices
    else No changes
        VM->>MainWindow: Show "No changes" notification
    end

    MainWindow->>User: Display updated device list
```

## Workflow 4: Error Handling - Device Without Serial Number

```mermaid
sequenceDiagram
    actor User
    participant ParseSvc as DeviceParsingService
    participant VM as MainViewModel
    participant Card as DeviceCard UI

    Note over ParseSvc: During enumeration/parsing
    ParseSvc->>ParseSvc: Extract VID/PID: Success
    ParseSvc->>ParseSvc: Extract Serial: null (missing)

    ParseSvc->>ParseSvc: Create UsbDevice with SerialNumber = null
    ParseSvc->>ParseSvc: Set HasSerialNumber = false

    ParseSvc-->>VM: Include device in Devices collection
    VM->>Card: Bind device data to card UI

    Note over Card: Data template checks HasSerialNumber
    alt HasSerialNumber == false
        Card->>Card: Disable "Copy for CrowdStrike" button
        Card->>Card: Show warning icon
        Card->>Card: Display error message inline
        Card->>User: Show "Serial number unavailable..."
    end

    Note over Card: "Copy for Helpdesk" remains enabled
    User->>Card: Can still copy basic device info
```
