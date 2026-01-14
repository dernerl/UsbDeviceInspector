# USB Device Inspector - User Personas

## Document Information

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2026-01-14 | PM Team | Initial user personas based on PRD requirements |

## Overview

This document defines the primary user personas for the USB Device Inspector application. These personas represent the target users whose needs, goals, and pain points drive product requirements and design decisions.

---

## Persona 1: IT Administrator - "Alex the IT Admin"

### Demographics

- **Name:** Alex Chen
- **Age:** 32
- **Role:** IT Systems Administrator
- **Company Size:** Mid-sized enterprise (500-2000 employees)
- **Location:** United States
- **Education:** Bachelor's degree in Information Technology

### Professional Background

- **Experience:** 8 years in IT support and systems administration
- **Team:** Works in a 5-person IT team supporting all corporate infrastructure
- **Responsibilities:**
  - Endpoint security management (CrowdStrike Falcon administration)
  - Device policy management and whitelisting
  - User access provisioning
  - Helpdesk escalation handling
  - Security compliance enforcement

### Technical Profile

- **Skill Level:** Intermediate to Advanced
- **Comfort with Command Line:** Moderate (can use PowerShell and CMD but prefers GUI tools)
- **Familiarity with:**
  - Windows Device Manager
  - CrowdStrike Falcon Console
  - Active Directory
  - PowerShell scripting (basic to intermediate)
  - USB device properties and identifiers (VID, PID)
- **Primary Tools:** Windows Server Admin Center, CrowdStrike Console, Remote Desktop, ServiceNow

### Goals & Motivations

**Primary Goals:**
1. Whitelist USB devices quickly and accurately for authorized users
2. Minimize time spent on repetitive device identification tasks
3. Reduce errors in device policy configuration
4. Enable end users to self-service where possible
5. Maintain security compliance while supporting business productivity

**Success Metrics:**
- Reduce device whitelisting time from 5-10 minutes to under 1 minute
- Eliminate manual transcription errors in CrowdStrike policies
- Decrease USB-related helpdesk tickets by 40%
- Maintain 100% accuracy in device identifier formatting

### Pain Points & Frustrations

**Current Workflow Challenges:**

1. **Manual Device Identification Process:**
   - Must request physical access to user's workstation or schedule remote session
   - Navigate Device Manager (requires knowing correct path: USB Controllers → Device Properties → Hardware IDs)
   - Manually parse VID and PID from cryptic hardware ID strings
   - Copy Serial Number from Device Instance Path (prone to typos)
   - Often requires elevated permissions which triggers security workflows

2. **CrowdStrike Format Conversion:**
   - Must manually format Combined ID: `VID_xxxx&PID_xxxx\SerialNumber`
   - Easy to make typos in hexadecimal values
   - Must double-check formatting before applying policy
   - No validation until policy is deployed

3. **Time Pressure:**
   - Average 5-10 minutes per device
   - Multiple devices per day (8-15 requests weekly)
   - Users waiting for approval to do their work
   - Competes with other IT priorities

4. **User Communication:**
   - Non-technical users can't provide complete device information
   - Vague descriptions: "my flash drive," "the blue USB stick"
   - Requires back-and-forth to get model/manufacturer info
   - Incomplete tickets require follow-up

**Quote:**
> "I spend way too much time on USB whitelisting. The process is simple but tedious - navigating Device Manager, finding the right properties, copying hex codes without mistakes. And users almost never give me the information I need upfront. A tool that just gives me the CrowdStrike format with one click would save me hours every week."

### Needs & Requirements

**Must Have:**
- One-click export of CrowdStrike-ready Combined ID format
- Works without admin privileges (Standard-User compatible)
- Fast device detection (under 3 seconds)
- Accurate parsing of VID, PID, Serial Number
- Clear visual confirmation when data is copied to clipboard

**Should Have:**
- Device manufacturer and model name displayed
- Refresh button to detect newly plugged devices
- Clean, scannable interface requiring no training

**Nice to Have:**
- Export device list to CSV for batch policy updates
- History of recently identified devices

### User Journey: Current State (Without Tool)

1. Receive helpdesk ticket: "User needs USB device whitelisted"
2. Schedule time with user or request physical device access
3. Open Device Manager on user's machine (requires elevation on locked-down workstations)
4. Navigate to USB Controllers → expand tree
5. Identify correct USB storage device among multiple entries
6. Right-click → Properties → Hardware IDs tab
7. Manually read and transcribe VID from string like `USB\VID_0781&PID_5581&REV_0100`
8. Switch to Details tab → Device Instance Path
9. Copy Serial Number from path like `USB\VID_0781&PID_5581\4C530001234567890123`
10. Open CrowdStrike Falcon Console
11. Navigate to Device Control policies
12. Manually type Combined ID: `VID_0781&PID_5581\4C530001234567890123`
13. Double-check for typos
14. Apply policy and test
15. **Total Time:** 5-10 minutes per device

### User Journey: Desired State (With USB Device Inspector)

1. Receive helpdesk ticket with device info from user (user ran the tool)
2. Open CrowdStrike Falcon Console
3. Paste Combined ID from ticket directly into policy
4. Apply policy
5. **Total Time:** 30 seconds per device

**OR** (if admin runs tool directly):

1. Plug in USB device (or have user plug it in during remote session)
2. Launch USB Device Inspector
3. Click "Copy for CrowdStrike"
4. Paste into Falcon Console
5. **Total Time:** 30-60 seconds

### Behavioral Traits

- **Task-Oriented:** Wants to complete whitelisting quickly and move to next task
- **Accuracy-Focused:** Fears making mistakes in security policies
- **Tool Pragmatist:** Prefers specialized tools over manual processes
- **Efficiency-Driven:** Values automation and time-saving utilities
- **Security-Conscious:** Cautious about installing untrusted software or requiring elevation

### Device & Environment

- **Primary Workstation:** Windows 10/11 Pro, domain-joined
- **Account Type:** Standard-User for daily work, admin account for elevated tasks
- **Network:** Corporate network with endpoint protection (CrowdStrike)
- **Remote Access:** Frequent remote desktop sessions to user workstations
- **Physical Access:** Intermittent access to user devices

---

## Persona 2: End User - "Maya the Marketing Manager"

### Demographics

- **Name:** Maya Rodriguez
- **Age:** 38
- **Role:** Marketing Manager
- **Company Size:** Mid-sized enterprise (500-2000 employees)
- **Location:** United States
- **Education:** Bachelor's degree in Communications

### Professional Background

- **Experience:** 12 years in marketing and brand management
- **Team:** Manages a 4-person marketing team
- **Responsibilities:**
  - Campaign planning and execution
  - Content creation and asset management
  - Vendor coordination
  - Budget management
  - Team leadership

### Technical Profile

- **Skill Level:** Basic to Intermediate (non-technical business user)
- **Comfort with Command Line:** None (has never used Command Prompt or PowerShell)
- **Familiarity with:**
  - Microsoft Office Suite (Word, Excel, PowerPoint, Outlook)
  - Adobe Creative Cloud (Photoshop, Illustrator - basic use)
  - Slack, Zoom, collaboration tools
  - Web browsers and cloud storage
- **Primary Tools:** Outlook, Chrome, Adobe apps, Slack, Asana
- **Technical Confidence:** Confident with business applications, intimidated by system-level tools

### Goals & Motivations

**Primary Goals:**
1. Get USB device approved quickly to meet project deadlines
2. Avoid delaying work while waiting for IT support
3. Provide IT with all information they need in one request
4. Minimize technical troubleshooting and IT interactions
5. Maintain productivity without security barriers

**Success Metrics:**
- Get USB device approved same-day instead of 1-3 days
- Submit complete helpdesk tickets that don't require follow-up
- Avoid scheduling IT desk visits or remote sessions

### Pain Points & Frustrations

**Current Workflow Challenges:**

1. **Information Gap:**
   - Doesn't know what device information IT needs
   - Can only describe device as "my Samsung USB drive" or "the red SanDisk"
   - IT asks for "VID and PID" - has no idea what those are
   - Doesn't know where to find device identifiers

2. **Incomplete Tickets:**
   - Submits ticket: "Please whitelist my USB drive"
   - IT responds: "We need device serial number and hardware IDs"
   - Doesn't know how to find this information
   - Leads to email back-and-forth or scheduled IT visit

3. **Time Delays:**
   - Submits ticket Monday morning
   - IT responds asking for more info Tuesday
   - Schedules remote session for Wednesday
   - Device approved Thursday
   - **Total Time:** 3-4 days for simple request

4. **Workflow Disruption:**
   - Can't access files on USB drive for campaign assets
   - Must delay vendor deliverables
   - Works around policy by emailing large files (slower, less secure)
   - Frustrated by security policies blocking legitimate work

5. **Technical Anxiety:**
   - Intimidated by Device Manager (complicated interface)
   - Afraid of breaking something if she clicks wrong option
   - Doesn't understand technical jargon in IT instructions
   - Feels helpless when encountering technical barriers

**Quote:**
> "When IT asks me for device IDs and serial numbers, I feel completely lost. I just know it's a Samsung USB drive. I wish there was a simple tool that could tell me what IT needs without having to navigate through complicated system menus or wait days for someone to remote into my computer."

### Needs & Requirements

**Must Have:**
- Extremely simple interface requiring zero technical knowledge
- One-click export of device information in format IT can use
- Clear labeling (avoid jargon like "VID" without explanation)
- Works without admin privileges on locked-down corporate PC
- Visual confirmation that information was copied

**Should Have:**
- Shows device friendly name/manufacturer so she can identify her device
- Explains what information is being provided and why IT needs it
- Copy button labeled clearly: "Copy for Helpdesk Ticket"

**Nice to Have:**
- Brief tooltip explaining what VID/PID/Serial Number means
- Confirmation message: "Device info copied - paste into your IT ticket"

### User Journey: Current State (Without Tool)

1. Plug in USB drive - blocked by CrowdStrike policy
2. See Windows notification: "Device blocked by security policy"
3. Open ServiceNow and submit ticket: "Please whitelist my Samsung USB drive"
4. Wait 4-8 hours for IT response
5. Receive email: "Please provide device VID, PID, and Serial Number"
6. Google "how to find USB device VID PID Windows 10"
7. Try to follow instructions to open Device Manager
8. Get confused by device tree and multiple USB entries
9. Reply to ticket: "I'm not sure how to find this information"
10. Schedule remote session with IT for next available slot (usually 1-2 days out)
11. IT connects remotely and identifies device
12. Device approved 3-4 days after initial request
13. **Total Time:** 3-4 days, multiple interruptions

### User Journey: Desired State (With USB Device Inspector)

1. Plug in USB drive - blocked by CrowdStrike policy
2. Launch USB Device Inspector (IT provided link in their helpdesk signature)
3. See her Samsung device listed with friendly name
4. Click "Copy for Helpdesk"
5. Open ServiceNow ticket
6. Paste device information into ticket description
7. Submit ticket with complete information
8. IT approves same day (or within hours)
9. **Total Time:** Same day approval, no remote sessions needed

### Behavioral Traits

- **Goal-Oriented:** Wants to complete marketing projects on deadline
- **Non-Technical:** Avoids system-level tools and technical interfaces
- **Self-Service Preference:** Prefers solving problems independently when possible
- **Clear Communication:** Values simple, jargon-free instructions
- **Time-Sensitive:** Works on tight deadlines, can't afford multi-day delays
- **Tool Hesitancy:** Reluctant to install unknown software without IT approval

### Device & Environment

- **Primary Workstation:** Windows 10/11 Pro, domain-joined, locked-down
- **Account Type:** Standard-User (no admin privileges)
- **Network:** Corporate network with strict endpoint protection
- **USB Usage:** Weekly - transferring marketing assets from vendors, backup of large files
- **IT Interaction:** Prefers self-service portals over phone/email support

### Relationship to Product

**Primary Use Case:** Self-service device identification for helpdesk tickets

**Value Proposition:**
- Empowers end users to provide complete device information to IT without technical knowledge
- Eliminates back-and-forth ticket communication
- Reduces USB approval time from days to hours
- Reduces IT burden by enabling self-service

**Adoption Barriers:**
- Must be approved/distributed by IT (won't download untrusted tools)
- Needs extremely simple interface (any complexity will cause abandonment)
- Must work without admin privileges
- Should not require installation or configuration

---

## Persona Comparison Matrix

| Dimension | IT Administrator (Alex) | End User (Maya) |
|-----------|------------------------|-----------------|
| **Technical Skill** | Intermediate/Advanced | Basic |
| **Command Line** | Moderate comfort | No experience |
| **USB Device Knowledge** | Understands VID/PID/Serial | Knows brand/model only |
| **Primary Goal** | Fast, accurate whitelisting | Quick device approval |
| **Time Pressure** | 8-15 requests/week | Occasional (2-4/month) |
| **Error Tolerance** | Very low (security impact) | N/A (not configuring) |
| **Preferred Interaction** | Direct tool usage | Copy-paste for IT |
| **Account Type** | Admin access available | Standard-User only |
| **Success Metric** | Time per device (<1 min) | Ticket resolution (<24h) |
| **Tool Discovery** | Active search for utilities | IT-recommended tools |

---

## Design Implications

### For IT Administrator Persona:

1. **CrowdStrike Format Priority:** Combined ID format must be pixel-perfect match to Falcon console requirements
2. **Speed Optimization:** Every click counts - minimize steps to clipboard export
3. **Accuracy Indicators:** Show exactly what will be copied before user clicks button
4. **Technical Language OK:** Can use terms like VID, PID, Serial Number without explanation
5. **Refresh Control:** Manual refresh button for control over device detection
6. **Standard-User Compatible:** Must work without elevation to support locked-down workstations

### For End User Persona:

1. **Friendly Naming:** Show manufacturer and friendly name prominently
2. **Clear Action Labels:** "Copy for Helpdesk" instead of technical jargon
3. **Visual Confirmation:** Obvious feedback when copy succeeds
4. **Zero Configuration:** Must work immediately without setup
5. **Simple Instructions:** In-app guidance in plain language
6. **Single-Purpose Focus:** Don't overwhelm with advanced features or options

### Shared Requirements:

- Works on Standard-User accounts (no elevation)
- Fast device detection (< 3 seconds)
- Single-click clipboard export
- Native Windows look and feel (trusted, professional)
- Minimal UI requiring no training

---

## User Research Recommendations

To validate and refine these personas, conduct the following research:

1. **User Interviews:** 5-8 interviews (3-4 IT admins, 2-4 end users) to validate pain points and workflows
2. **Contextual Inquiry:** Observe 2-3 IT admins performing USB whitelisting in real environment
3. **Survey:** Broader survey (20-30 IT admins) on device management tools and pain points
4. **Usability Testing:** Test prototype with 3 IT admins and 3 end users for workflow validation
5. **CrowdStrike Community Research:** Review forums/Reddit for user complaints about device whitelisting workflows

---

## Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 2026-01-14 | Initial persona creation based on PRD requirements | PM Team |