# Introduction

This document outlines the overall project architecture for **USB Device Inspector**, including backend systems, service layer architecture, Windows API integration, and non-UI specific concerns. Its primary goal is to serve as the guiding architectural blueprint for AI-driven development, ensuring consistency and adherence to chosen patterns and technologies.

**Relationship to Frontend Architecture:**
The project includes a significant WinUI3 user interface. A separate Frontend Architecture Document will detail the frontend-specific design (XAML views, ViewModels, UI components) and MUST be used in conjunction with this document. Core technology stack choices documented herein (see "Tech Stack") are definitive for the entire project, including frontend components.

## Starter Template or Existing Project

**N/A** - This is a greenfield project using the official **Microsoft Windows App SDK WinUI3 template** (`wasdk-winui-cs`) as the project scaffolding foundation. The template provides:

- Pre-configured WinUI3 project structure with App.xaml and MainWindow.xaml
- Windows App SDK NuGet packages and build configuration
- Basic application lifecycle management
- XAML/C# integration scaffolding

The architecture will build upon this template's conventions while adding custom service layer patterns, MVVM implementation, and domain-specific business logic.

## Change Log

| Date | Version | Description | Author |
|------|---------|-------------|--------|
| 2026-01-14 | 1.0 | Initial architecture document draft | Winston (Architect) |
