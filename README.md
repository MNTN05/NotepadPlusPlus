# 📝 NotepadPlusPlus

> A feature-rich, multi-tab text editor for Windows — built from scratch with C# and WPF to demonstrate clean MVVM architecture and modern .NET practices.

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/MNTN05/NotepadPlusPlus)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)
[![Version](https://img.shields.io/badge/version-1.0.0-orange)](https://github.com/MNTN05/NotepadPlusPlus/releases)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)

---

## 🗂️ Table of Contents

- [About the Project](#about-the-project)
- [Key Features](#key-features)
- [Tech Stack](#tech-stack)
- [Architecture & Design](#architecture--design)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Challenges & Learnings](#challenges--learnings)
- [Future Improvements](#future-improvements)
- [Contact](#contact)

---

## About the Project

**NotepadPlusPlus** is a desktop text editor that reimagines the classic Notepad experience with a tabbed interface, integrated folder explorer, and powerful search/replace capabilities — all built on a strict MVVM foundation.

The project was built to demonstrate real-world application of **separation of concerns** in a Windows desktop environment: every unit of behaviour (file I/O, dialogs, clipboard) lives behind an interface, making the application testable and extensible without touching the UI layer.

**Value proposition:** This is not a tutorial clone. It is a production-structured WPF application that showcases how enterprise-grade desktop software is architected — with source-generated commands, attached behaviours, and a clean service abstraction layer.

---

## Key Features

- **Multi-tab editing** — open an unlimited number of files simultaneously; tabs display a visual dirty indicator when unsaved changes are present.
- **Full file lifecycle management** — New, Open, Save, Save As, Close (single tab), and Close All, each with an unsaved-changes prompt to prevent data loss.
- **Find & Replace** — forward and backward search with case-insensitive matching, replacements for the current occurrence or all occurrences at once, and an **"All Tabs" mode** that searches across every open document.
- **Folder Explorer sidebar** — hierarchical tree view of the file system with context-menu actions: create a new file, copy a path, copy/paste entire folder trees.
- **Text manipulation** — convert selected text to UPPERCASE or lowercase, remove all empty lines, and jump to a specific line number.
- **Read-only mode** — toggle per-tab write protection with a single click or menu action.
- **Attached Behaviour for selection sync** — a custom `TextBoxBehavior` attached property keeps `SelectionStart`, `SelectionLength`, and `CaretIndex` fully bindable in the ViewModel, a non-trivial WPF challenge solved without code-behind.
- **Keyboard shortcuts** — `Ctrl+N/O/S/Shift+S/F/H` wired directly to commands via `InputBindings`.

---

## Tech Stack

### 🖥️ UI Framework

| Technology | Why it was chosen |
|---|---|
| **WPF (Windows Presentation Foundation)** | Provides a mature, hardware-accelerated XAML rendering pipeline with first-class data binding — ideal for a data-driven desktop UI where every control state is driven by ViewModel properties. |
| **XAML** | Keeps the view layer declarative and entirely separate from business logic; templates, styles, and triggers replace imperative code-behind. |

### ⚙️ Application Layer

| Technology | Why it was chosen |
|---|---|
| **C# 12 / .NET 8.0** | The latest stable LTS runtime brings performance improvements, nullable reference types for safer null handling, and C# 12 features such as collection expressions. |
| **CommunityToolkit.Mvvm 8.2.2** | Source-generator–based toolkit that eliminates MVVM boilerplate: `[ObservableProperty]` generates `INotifyPropertyChanged` implementations at compile time, and `[RelayCommand]` generates `ICommand` wrappers — resulting in lean, readable ViewModels with zero hand-written plumbing. |

### 🏗️ Infrastructure / DevOps

| Technology | Why it was chosen |
|---|---|
| **MSBuild / .NET SDK** | Standard build toolchain; the project targets `net8.0-windows` so it can be built and published with a single `dotnet publish` command for self-contained deployment. |
| **Git / GitHub** | Version control and remote hosting; the branching strategy supports feature isolation and pull-request–based code review. |

---

## Architecture & Design

### MVVM (Model-View-ViewModel)

The entire application follows the **MVVM** pattern enforced by strict layer separation:

```
┌─────────────────────────────────────────────┐
│  View  (XAML + minimal code-behind)          │  Data binding ↕
├─────────────────────────────────────────────┤
│  ViewModel  (ObservableObject + RelayCommand)│  Calls ↓
├─────────────────────────────────────────────┤
│  Service Layer  (IFileService, IDialogService│
│                  IClipboardService)          │  Implementations ↓
├─────────────────────────────────────────────┤
│  OS / Win32 APIs  (File I/O, WinForms dialogs│
│                    Clipboard)                │
└─────────────────────────────────────────────┘
```

### Design Patterns in Use

- **MVVM** — primary architectural pattern; Views have no business logic.
- **Service Abstraction / Dependency Injection** — `IFileService`, `IDialogService`, and `IClipboardService` are injected manually via the `App.OnStartup` composition root, making every ViewModel unit-testable with mock services.
- **Attached Behaviour** (`TextBoxBehavior`) — extends WPF's `TextBox` with bindable `SelectionStart`, `SelectionLength`, and `CaretIndex` properties without subclassing, following the WPF Attached Property pattern.
- **Value Converters** — `BoolToVisibilityConverter`, `InverseBoolToVisibilityConverter`, and `BoolToColorConverter` keep conditional UI logic out of both View and ViewModel.
- **Command Pattern** — every user action is represented as an `IRelayCommand`, enabling declarative binding in XAML and clean testability.

---

## Getting Started

### Prerequisites

- **Operating System:** Windows 10 / 11 (WPF is Windows-only)
- **Runtime:** [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later
- **IDE (optional):** [Visual Studio 2022](https://visualstudio.microsoft.com/) (v17.8+) or [JetBrains Rider](https://www.jetbrains.com/rider/)

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/MNTN05/NotepadPlusPlus.git
   cd NotepadPlusPlus
   ```

2. **Restore NuGet packages**

   ```bash
   dotnet restore
   ```

3. **Build the project**

   ```bash
   dotnet build
   ```

### Environment Variables

This project does not require any environment variables or configuration files. All settings are resolved at runtime via the Windows registry and standard OS dialogs.

### Running the Application

**From the command line:**

```bash
dotnet run --project NotepadPlusPlus.csproj
```

**From Visual Studio:**

Open `NotepadPlusPlus.sln` and press **F5** (Debug) or **Ctrl+F5** (Run without debugger).

**Self-contained publish (single executable):**

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The output executable will be located in `bin/Release/net8.0-windows/win-x64/publish/`.

---

## Usage

> 📸 _Screenshots and GIFs demonstrating the application will be added here._

| Feature | Keyboard Shortcut |
|---|---|
| New file | `Ctrl+N` |
| Open file | `Ctrl+O` |
| Save | `Ctrl+S` |
| Save As | `Ctrl+Shift+S` |
| Find | `Ctrl+F` |
| Replace | `Ctrl+H` |

_Suggested screenshots to add:_
- Main window with multiple open tabs
- Find & Replace dialog with an active multi-tab search
- Folder Explorer sidebar open alongside a document

---

## Challenges & Learnings

### Two-Way Binding of `TextBox` Selection State in WPF

**The challenge:** WPF's built-in `TextBox` control does not expose `SelectionStart`, `SelectionLength`, or `CaretIndex` as dependency properties, which means they cannot be bound to ViewModel properties out of the box. The Find & Replace feature required the ability to highlight a search result (set selection) and scroll to its position purely from the ViewModel — without touching the code-behind.

**The solution:** A custom **Attached Behaviour** (`TextBoxBehavior`) was implemented. It registers three attached dependency properties on `TextBox` and hooks into the `SelectionChanged` event to propagate state changes back to the ViewModel. A boolean guard (`IsUpdating`) prevents re-entrant updates when the property change originates from the ViewModel itself.

```csharp
// Setting the caret and scrolling to the matched line — all from the ViewModel:
public void Highlight(int start, int length)
{
    CaretIndex    = start;
    SelectionStart  = start;
    SelectionLength = length;
}
```

**The learning:** WPF's attached property system is a powerful extension mechanism. When the framework does not expose the binding surface you need, the correct pattern is to reach for an attached behaviour rather than falling back to code-behind or tight View-ViewModel coupling.

---

## Future Improvements

- [ ] **Syntax highlighting** — integrate a lightweight lexer to colorize keywords for common languages (C#, Python, JSON, XML).
- [ ] **Line number gutter** — display line numbers in a fixed-width panel alongside the editor area.
- [ ] **Word wrap toggle** — per-tab setting to enable or disable line wrapping.
- [ ] **Recent files list** — persist the last N opened files (using `IsolatedStorage` or a JSON settings file) and surface them in the File menu.
- [ ] **Drag-and-drop tab reordering** — allow tabs to be repositioned via drag and drop.
- [ ] **Plugin / extension system** — expose a minimal API so that additional commands can be registered at startup without modifying the core codebase.
- [ ] **Unit & integration tests** — add an xUnit test project with mock implementations of `IFileService`, `IDialogService`, and `IClipboardService` to verify ViewModel logic in isolation.
- [ ] **Auto-save** — configurable interval-based background save to a temporary file to protect against data loss.

---

## Contact

**Munteanu David-Gabriel**

- 📧 Email: [david-gabriel.munteanu@student.unitbv.ro](mailto:david-gabriel.munteanu@student.unitbv.ro)
- 🎓 Group: 10LF342 — Transilvania University of Brașov
- 🐙 GitHub: [github.com/MNTN05](https://github.com/MNTN05)

---

_Built with ❤️ using C# and WPF_
