# Goals and Background Context

## Goals
- Enable IT administrators to identify USB storage devices and extract VID, PID, and Serial Number within seconds
- Reduce USB device whitelisting time from 5-10 minutes to under 60 seconds per device
- Provide CrowdStrike Falcon Device Control-compatible output formats eliminating manual formatting errors
- Operate reliably on Standard-User accounts without requiring elevation or administrator privileges
- Deliver card-based UI requiring zero training or documentation for basic workflow completion
- Enable end users to self-identify their USB devices and communicate complete device information to IT support
- Reduce helpdesk ticket incompleteness by providing standardized device identification for end-user whitelisting requests

## Background Context

Enterprise IT administrators managing CrowdStrike Falcon Device Control face significant operational friction when whitelisting USB storage devices. The current manual process requires command-line expertise, elevated permissions, and manual transcription of device identifiers (VID, PID, Serial Number) into CrowdStrike's specific Combined ID format. This 5-10 minute workflow per device is error-prone, creates security policy conflicts, and delays user productivity.

Additionally, end users requesting USB device whitelisting often cannot provide complete device information beyond descriptions like "my flash drive" or "the blue USB stick," forcing helpdesk agents to schedule in-person visits or attempt to guide non-technical users through Device Manager or command-line tools. This extends ticket resolution time from hours to days and creates frustration for both employees and support staff.

The USB Device Inspector solves this by leveraging Windows Runtime (WinRT) APIs to enumerate USB devices from Standard-User accounts, automatically parsing device identifiers and formatting them for direct use in workflows. For IT administrators, it enables immediate clipboard export in CrowdStrike-ready format for direct paste into the Falcon console. For end users, it provides all necessary device information in a readable format that can be included in helpdesk tickets, eliminating incomplete requests and accelerating the approval workflow.

## User Personas

This PRD is built around two primary user personas:

1. **Alex the IT Administrator** - Mid-level IT admin managing CrowdStrike Falcon Device Control, performing 8-15 USB whitelisting requests weekly. Seeks speed, accuracy, and Standard-User compatibility to streamline device identification workflow from 5-10 minutes to under 60 seconds.

2. **Maya the Marketing Manager** - Non-technical end user who needs USB devices approved for work. Lacks knowledge of device identifiers (VID/PID/Serial Number) and struggles with incomplete helpdesk tickets, leading to 3-4 day approval cycles. Seeks simple self-service tool to provide IT with complete device information.

For detailed persona descriptions including goals, pain points, user journeys, and design implications, see [user-personas.md](user-personas.md).

## Change Log

| Date | Version | Description | Author |
|------|---------|-------------|--------|
| 2026-01-13 | 0.1 | Initial PRD draft based on Project Brief | John (PM Agent) |
| 2026-01-13 | 0.2 | Added end-user/helpdesk scenario to goals and background | John (PM Agent) |
| 2026-01-13 | 1.0 | Completed all 10 epics with 81 user stories, added Next Steps section | John (PM Agent) |
| 2026-01-14 | 1.1 | Added detailed user personas (Alex the IT Admin, Maya the Marketing Manager) | PM Team |
| 2026-01-14 | 1.2 | Added comprehensive architecture diagrams (ASCII) and technical guidance | PM Team |
