# Next Steps

## UX Expert Prompt

You are the UX Expert for the USB Device Inspector project. Please review the Product Requirements Document (PRD) located at `docs/prd.md` and create a comprehensive UX/UI design specification.

**Your objectives:**

1. **Review the PRD** - Understand the goals, requirements, user interface design goals, and all epic/story details
2. **Design the card-based UI** - Create detailed specifications for:
   - Device card layout and visual hierarchy
   - Typography, spacing, and Fluent Design System styling
   - Button placement and interaction states
   - Empty state and error state designs
   - Accessibility patterns (focus indicators, screen reader annotations)
3. **Define interaction patterns** - Specify:
   - Copy button behavior and visual feedback
   - Refresh button interaction
   - Keyboard navigation flow
   - Loading states during device enumeration
4. **Create XAML templates** - Provide reference XAML code for:
   - Device card DataTemplate
   - Main window layout structure
   - Empty state view
   - Error state display patterns

**Deliverables:**
- UX/UI specification document in `docs/ux-design.md`
- XAML reference templates or mockup code
- Accessibility annotation guide for developers

**Key constraints from PRD:**
- WCAG 2.1 Level AA compliance mandatory
- Fluent Design System styling (native Windows 10/11 look)
- Zero-configuration, zero-learning-curve UI
- 800x600 minimum window size
- Single-screen application (no navigation)

Please enter UX design mode and begin by reviewing the PRD to understand the complete context.

---

## Architect Prompt

You are the Software Architect for the USB Device Inspector project. Please review the Product Requirements Document (PRD) located at `docs/prd.md` and create a comprehensive technical architecture specification.

**Your objectives:**

1. **Review the PRD** - Understand goals, functional/non-functional requirements, technical assumptions, and all epic/story details
2. **Design the service architecture** - Create detailed specifications for:
   - `DeviceEnumerationService` - Windows.Devices.Enumeration API integration
   - `DeviceParsingService` - VID/PID/Serial Number extraction logic
   - `FormatService` - CrowdStrike and helpdesk format generation
   - `ClipboardExportService` - Windows clipboard integration
   - `MainViewModel` - MVVM pattern with CommunityToolkit.Mvvm
3. **Define data models** - Specify:
   - `UsbDevice` class with all properties (VID, PID, SerialNumber, FriendlyName, Manufacturer, IsValid, ErrorMessage)
   - View model properties and observable collections
4. **Specify API integration patterns** - Detail:
   - Async/await patterns for device enumeration
   - Error handling strategies
   - Device filtering logic (exclude internal SD card readers)
5. **Define testing approach** - Specify:
   - Unit test structure and mocking patterns
   - Integration test scenarios with real Windows APIs
   - Test data sets for edge cases

**Deliverables:**
- Architecture specification document in `docs/architecture.md`
- Class diagrams or structure definitions
- API integration patterns and code examples
- Testing strategy document

**Key constraints from PRD:**
- .NET 8.0 + WinUI3 stack
- VS Code + .NET CLI development workflow
- Standard-User permissions (no elevation required)
- Async operations for UI responsiveness (NFR2)
- 80%+ unit test coverage for service layer
- Monolithic desktop application (no microservices)

**Critical technical requirements:**
- Device enumeration must complete within 3 seconds (NFR1)
- Cold start time under 2 seconds (NFR6)
- 90%+ Device Instance Path parsing success rate (NFR12)
- 100% CrowdStrike format accuracy (NFR9)

Please enter architecture mode and begin by reviewing the PRD to understand the complete technical context and requirements.

---

**PRD Complete - Ready for Handoff to UX Expert and Architect** ✓

