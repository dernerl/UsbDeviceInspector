# Epic 6: WinUI3 User Interface & MVVM Implementation

**Epic Goal:** Build the card-based user interface using WinUI3 and implement the MVVM pattern with CommunityToolkit.Mvvm, delivering a clean, scannable interface where users immediately see all connected USB devices as individual cards with prominent action buttons. This epic delivers the zero-training-curve user experience where users understand what they're looking at and what actions they can take within 2-3 seconds of opening the application, with single-click export and instant feedback for all user actions.

## Story 6.1: Create MainViewModel with Device Collection

As a **developer**,
I want **a ViewModel to manage the device list and user interactions**,
so that **UI logic is separated from business logic following MVVM pattern**.

**Acceptance Criteria:**

1. `MainViewModel` class created in `ViewModels/` folder
2. ViewModel inherits from `ObservableObject` (CommunityToolkit.Mvvm)
3. ViewModel includes observable property `ObservableCollection<UsbDevice> Devices`
4. ViewModel includes properties:
   - `bool IsLoading` (indicates enumeration in progress)
   - `bool IsEmpty` (true when no devices connected)
   - `string StatusMessage` (displays messages to user)
5. ViewModel includes commands (using `RelayCommand`):
   - `RefreshCommand` (triggers device re-enumeration)
6. ViewModel constructor accepts dependency injection parameters:
   - `DeviceEnumerationService`
   - `FormatService`
   - `ClipboardExportService`
7. Unit tests validate ViewModel construction and property initialization
8. ViewModel includes XML documentation for public members

## Story 6.2: Implement LoadDevices Async Method

As a **developer**,
I want **ViewModel to load devices asynchronously on initialization**,
so that **UI populates with device data on application launch** (per FR12).

**Acceptance Criteria:**

1. ViewModel includes async method `Task LoadDevicesAsync()`
2. Method sets `IsLoading = true` before enumeration, `IsLoading = false` after completion
3. Method calls `DeviceEnumerationService.EnumerateDevicesAsync()` to fetch devices
4. Method populates `Devices` collection with results
5. Method sets `IsEmpty = true` if no devices found, `IsEmpty = false` otherwise
6. Method updates `StatusMessage` with result (e.g., "Found 3 USB devices" or "No USB devices connected")
7. Method handles exceptions gracefully (logs error, sets StatusMessage to error text)
8. Unit tests validate:
   - IsLoading flag set correctly during operation
   - Devices collection populated after load
   - IsEmpty flag set correctly
   - Exception handling behavior
9. Manual testing: launch app, verify devices appear automatically

## Story 6.3: Implement RefreshCommand with User Feedback

As an **IT administrator**,
I want **a Refresh button that re-enumerates devices with visual feedback**,
so that **I can detect newly connected devices without restarting the app** (per FR11).

**Acceptance Criteria:**

1. `RefreshCommand` implemented as `AsyncRelayCommand` (CommunityToolkit.Mvvm)
2. Command calls `LoadDevicesAsync()` when executed
3. Command disabled during enumeration (IsLoading = true) to prevent multiple simultaneous refreshes
4. StatusMessage updated to "Refreshing devices..." during operation
5. StatusMessage updated to result after completion (e.g., "Found 3 USB devices" or "No USB devices connected")
6. Unit tests validate:
   - Command execution calls LoadDevicesAsync
   - Command disabled during loading
   - StatusMessage updated correctly
7. Manual testing: click Refresh button, verify UI shows loading state then updated device list

## Story 6.4: Design Device Card UI in XAML

As a **developer**,
I want **XAML templates for device cards with clear information hierarchy**,
so that **users can quickly scan and identify their USB devices** (per UI Design Goals).

**Acceptance Criteria:**

1. MainWindow.xaml updated with ItemsRepeater displaying device cards
2. Device card template includes:
   - Device icon (USB icon from Fluent icon set)
   - FriendlyName (large, bold text - primary information)
   - Manufacturer (secondary text, smaller font)
   - Serial Number (monospace font, prominent)
   - VID/PID (smaller text, labeled "Technical Details")
   - Action buttons: "Copy for CrowdStrike" and "Copy for Helpdesk"
3. Card uses Fluent Design System styling:
   - Card background with subtle shadow/elevation
   - Rounded corners (CornerRadius)
   - Padding for breathing room
4. Cards displayed in vertical scrollable list (ScrollViewer with ItemsRepeater)
5. Visual hierarchy emphasizes FriendlyName and Serial Number
6. Card layout responsive to window resizing (maintains readability)
7. Manual testing: launch app, verify cards display correctly with test data

## Story 6.5: Implement Copy for CrowdStrike Command

As an **IT administrator**,
I want **a button on each device card to copy CrowdStrike format to clipboard**,
so that **I can export device identifiers with a single click** (per FR8).

**Acceptance Criteria:**

1. ViewModel includes command `CopyCrowdStrikeCommand` accepting `UsbDevice` parameter
2. Command implemented as `AsyncRelayCommand<UsbDevice>`
3. Command calls export service `ExportCrowdStrikeFormatAsync(device)`
4. Command updates StatusMessage to "Copied to clipboard" on success
5. Command updates StatusMessage to error message on failure
6. Button in device card XAML bound to CopyCrowdStrikeCommand with CommandParameter=device
7. Button disabled when device.IsValid = false (cannot export invalid devices)
8. Unit tests validate:
   - Command execution calls export service
   - StatusMessage updated correctly
   - Command disabled for invalid devices
9. Manual testing: click button, paste into Notepad, verify CrowdStrike format appears

## Story 6.6: Implement Copy for Helpdesk Command

As an **end user**,
I want **a button on each device card to copy helpdesk format to clipboard**,
so that **I can include device information in support tickets with a single click** (per FR9).

**Acceptance Criteria:**

1. ViewModel includes command `CopyHelpdeskCommand` accepting `UsbDevice` parameter
2. Command implemented as `AsyncRelayCommand<UsbDevice>`
3. Command calls export service `ExportHelpdeskFormatAsync(device)`
4. Command updates StatusMessage to "Copied for helpdesk" on success
5. Button in device card XAML bound to CopyHelpdeskCommand with CommandParameter=device
6. Button enabled even for invalid devices (helpdesk format supports partial data)
7. Unit tests validate command execution and status messages
8. Manual testing: click button, paste into Notepad, verify helpdesk format appears

## Story 6.7: Add Visual Feedback for Clipboard Operations

As a **user**,
I want **immediate visual confirmation when data is copied to clipboard**,
so that **I know the copy operation succeeded** (per FR10).

**Acceptance Criteria:**

1. Copy buttons show visual state change on click:
   - Button content changes to "Copied!" for 2 seconds
   - Button background briefly highlights with accent color
   - Button returns to normal state after timeout
2. StatusMessage at top of window displays success message (e.g., "Copied to clipboard")
3. StatusMessage auto-clears after 5 seconds (or next user action)
4. Visual feedback works for both CrowdStrike and Helpdesk copy operations
5. Manual testing validates:
   - Click copy button
   - Button text changes to "Copied!"
   - StatusMessage appears
   - UI returns to normal state after timeout
6. Feedback visible and noticeable (no subtle animations that users might miss)

## Story 6.8: Implement Empty State View

As a **user**,
I want **clear guidance when no USB devices are connected**,
so that **I know what to do next** (per FR15).

**Acceptance Criteria:**

1. MainWindow.xaml includes empty state template displayed when `IsEmpty = true`
2. Empty state includes:
   - Large USB icon (visually indicates what's missing)
   - Message: "No USB devices connected"
   - Instructions: "Plug in a USB storage device and click Refresh"
   - Refresh button (prominent, centered)
3. Empty state replaces device list (not shown alongside empty list)
4. Empty state uses Fluent Design styling (centered, readable)
5. Manual testing validates:
   - Launch app with no USB devices connected
   - Empty state appears with message and Refresh button
   - Plug in USB device, click Refresh
   - Device list replaces empty state

## Story 6.9: Display Inline Errors for Invalid Devices

As a **user**,
I want **devices with parsing errors to show error messages inline on the card**,
so that **I understand why some devices cannot be exported** (per FR14).

**Acceptance Criteria:**

1. Device card template includes error banner (visible when `device.IsValid = false`)
2. Error banner displays `device.ErrorMessage` text
3. Error banner uses warning color scheme (yellow/orange background, dark text)
4. Error banner positioned prominently on card (near top, above action buttons)
5. Copy for CrowdStrike button disabled for invalid devices (with tooltip explaining why)
6. Copy for Helpdesk button remains enabled (partial data export)
7. Unit tests validate error display binding
8. Manual testing validates:
   - Device with missing serial number shows error banner
   - Error message is readable and actionable
   - CrowdStrike button disabled, Helpdesk button enabled

## Story 6.10: Implement Loading State UI

As a **user**,
I want **visual indication when the app is enumerating devices**,
so that **I know the app is working and not frozen** (per NFR2).

**Acceptance Criteria:**

1. Loading indicator displayed when `IsLoading = true`
2. Loading UI includes:
   - ProgressRing (spinning animation)
   - StatusMessage: "Loading USB devices..."
3. Loading UI overlays or replaces device list (device list not visible during loading)
4. Loading indicator uses Fluent Design styling
5. Refresh button disabled during loading (or shows loading state)
6. Manual testing validates:
   - Launch app, see loading indicator briefly
   - Click Refresh, see loading indicator during enumeration
   - Loading completes within 3 seconds (per NFR1)

## Story 6.11: Set Window Properties and Constraints

As a **user**,
I want **the application window to have appropriate size and title**,
so that **it's usable and identifiable in my taskbar**.

**Acceptance Criteria:**

1. MainWindow.xaml Title set to "USB Device Inspector"
2. Window minimum size set to 800x600 (per UI Design Goals)
3. Window default size set to 900x700 (comfortable viewing)
4. Window resizable (users can expand to see more cards)
5. Window icon set to USB device icon (if available, or Windows default)
6. Manual testing validates:
   - Window opens at default size
   - Window cannot be resized smaller than 800x600
   - Window title visible in taskbar

## Story 6.12: Integrate ViewModel with MainWindow Code-Behind

As a **developer**,
I want **MainWindow to instantiate and bind to MainViewModel**,
so that **MVVM pattern is properly implemented with dependency injection**.

**Acceptance Criteria:**

1. MainWindow.xaml.cs constructor creates/receives MainViewModel instance
2. MainWindow DataContext set to MainViewModel
3. MainViewModel LoadDevicesAsync() called in MainWindow OnNavigatedTo or Loaded event
4. Services (DeviceEnumerationService, etc.) instantiated and injected into ViewModel
5. Application startup logic configured in App.xaml.cs
6. Manual testing validates:
   - Launch application
   - Devices load automatically on startup
   - UI interactions trigger ViewModel commands
7. Code-behind file minimal (only initialization, no business logic)
