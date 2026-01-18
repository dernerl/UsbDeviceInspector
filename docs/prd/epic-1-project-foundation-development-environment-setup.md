# Epic 1: Project Foundation & Development Environment Setup

**Epic Goal:** Establish the complete development infrastructure including Git repository, .NET solution structure, VS Code configuration, CI/CD pipeline, and initial project scaffolding to enable efficient MVP development. Deliver a "Hello World" level application that can be built, run, debugged, and deployed through the full pipeline, validating that all foundational infrastructure is working correctly.

## Story 1.1: Initialize Git Repository and Project Structure

As a **developer**,
I want **a properly structured Git repository with the monorepo layout**,
so that **I can organize code, documentation, and configuration files according to the architectural plan**.

**Acceptance Criteria:**

1. GitHub repository created with name `UsbDeviceInspector` and MIT License
2. Repository README.md includes project title, brief description, and placeholder for setup instructions
3. Directory structure matches Technical Assumptions specification:
   - `src/` (source code root)
   - `docs/` (documentation including PRD)
   - `.vscode/` (VS Code configuration)
   - `.github/workflows/` (CI/CD pipelines)
4. `.gitignore` file configured for .NET projects (excludes `bin/`, `obj/`, `.vs/`, etc.)
5. Repository cloned locally and verified accessible

## Story 1.2: Create .NET Solution and WinUI3 Project

As a **developer**,
I want **a .NET 8.0 solution with WinUI3 application project scaffolded**,
so that **I have the foundational application structure to build upon**.

**Acceptance Criteria:**

1. Windows App SDK templates installed via `dotnet new install Microsoft.WindowsAppSDK.Templates`
2. .NET solution created: `UsbDeviceInspector.sln`
3. WinUI3 project created: `src/UsbDeviceInspector/UsbDeviceInspector.csproj` using `wasdk-winui-cs` template
4. Project targets .NET 8.0 and Windows 10 SDK version 10.0.19041.0 or later
5. Project builds successfully via `dotnet build`
6. Application runs and displays default WinUI3 window via `dotnet run`
7. Solution file references the WinUI3 project

## Story 1.3: Create xUnit Test Project

As a **developer**,
I want **an xUnit test project with necessary testing dependencies**,
so that **I can write unit and integration tests from the beginning**.

**Acceptance Criteria:**

1. xUnit test project created: `src/UsbDeviceInspector.Tests/UsbDeviceInspector.Tests.csproj`
2. Test project targets .NET 8.0
3. Test project includes NuGet packages:
   - xUnit (latest stable)
   - FluentAssertions (latest stable)
   - NSubstitute (latest stable)
   - xunit.runner.visualstudio (for test discovery)
4. Test project references main UsbDeviceInspector project
5. Solution file references the test project
6. Sample "hello world" test exists and passes: `dotnet test` succeeds with 1 passing test
7. Test results displayed in console output

## Story 1.4: Configure VS Code for Debugging

As a **developer**,
I want **VS Code configured with launch and task configurations**,
so that **I can debug the WinUI3 application with F5 and run tests easily**.

**Acceptance Criteria:**

1. `.vscode/launch.json` exists with launch configuration for WinUI3 app:
   - Type: `coreclr`
   - Program path points to built executable
   - Pre-launch task configured to build project
2. `.vscode/tasks.json` exists with tasks:
   - Build task (`dotnet build`)
   - Test task (`dotnet test`)
   - Run task (`dotnet run`)
3. C# Dev Kit extension listed in `.vscode/extensions.json` as recommended
4. Developer can press F5 in VS Code and application launches with debugger attached
5. Breakpoints set in `App.xaml.cs` are hit during debugging
6. Test task runs from VS Code Task Runner and displays results

## Story 1.5: Add Core NuGet Dependencies

As a **developer**,
I want **CommunityToolkit.Mvvm and other foundational dependencies installed**,
so that **I can implement MVVM pattern and other architectural requirements**.

**Acceptance Criteria:**

1. CommunityToolkit.Mvvm (version 8.0+) added to UsbDeviceInspector project
2. Microsoft.WindowsAppSDK (version 1.5+) verified in project (should be from template)
3. All dependencies restored via `dotnet restore`
4. Project builds successfully with all dependencies
5. No dependency version conflicts or warnings in build output

## Story 1.6: Implement GitHub Actions CI/CD Pipeline

As a **developer**,
I want **a GitHub Actions workflow that builds and tests the solution automatically**,
so that **every push and pull request is validated before merging**.

**Acceptance Criteria:**

1. Workflow file exists: `.github/workflows/build.yml`
2. Workflow triggers on:
   - Push to `main` branch
   - Pull requests targeting `main`
3. Workflow steps include:
   - Checkout repository
   - Setup .NET 8.0 SDK
   - Install Windows App SDK templates
   - Restore dependencies
   - Build solution
   - Run tests
4. Workflow runs on `windows-latest` runner (required for WinUI3)
5. Build artifacts created (executable and DLLs) and uploaded as workflow artifacts
6. Workflow completes successfully on test push to main branch
7. Badge added to README.md showing build status

## Story 1.7: Create Initial Folder Structure and Services

As a **developer**,
I want **the project organized with Views, ViewModels, Models, and Services folders**,
so that **code is logically organized according to MVVM architecture**.

**Acceptance Criteria:**

1. Folders created in `src/UsbDeviceInspector/`:
   - `Views/` (XAML views)
   - `ViewModels/` (view models)
   - `Models/` (data models)
   - `Services/` (business logic services)
   - `Assets/` (icons, images - may exist from template)
2. Existing `MainWindow.xaml` moved to `Views/` folder
3. Namespace references updated in XAML and code-behind to reflect new folder structure
4. Application builds and runs successfully after reorganization
5. No broken references or runtime errors

## Story 1.8: Create "Hello Device Inspector" Validation Screen

As a **developer**,
I want **a simple UI displaying "USB Device Inspector - Ready" message**,
so that **I can verify the complete development pipeline works end-to-end**.

**Acceptance Criteria:**

1. MainWindow.xaml updated with:
   - TextBlock displaying "USB Device Inspector"
   - Subtitle TextBlock displaying "Development environment ready"
   - Simple Fluent Design styling (centered, readable typography)
2. Application launches and displays the message correctly
3. Window title set to "USB Device Inspector"
4. Window minimum size set to 800x600 per NFR requirements
5. Debug build runs via VS Code F5 successfully
6. Release build created via `dotnet build -c Release -p:Platform=x64` successfully
7. GitHub Actions workflow builds and tests successfully
8. Application can be manually tested by running executable from `bin/Debug/` folder

## Story 1.9: Document Development Environment Setup

As a **new developer joining the project**,
I want **comprehensive setup instructions in the README**,
so that **I can configure my environment and start developing quickly**.

**Acceptance Criteria:**

1. README.md includes sections:
   - Project description and goals (brief, 2-3 sentences)
   - Prerequisites (Windows 10/11, .NET 8.0 SDK, VS Code)
   - VS Code extensions required (C# Dev Kit, C#, GitLens)
   - Step-by-step setup instructions:
     - Clone repository
     - Install Windows App SDK templates
     - Restore dependencies
     - Build solution
     - Run application
     - Run tests
   - How to debug in VS Code (F5 to launch)
   - How to run CI/CD locally (dotnet commands)
   - Contributing guidelines placeholder
2. Instructions tested by following them on a clean machine/VM (or documented as validated)
3. Common troubleshooting section with known issues (e.g., Windows SDK version mismatches)
4. Links to external documentation (WinUI3 docs, .NET 8 docs)
