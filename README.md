<div align="center">

# 📝 NotepadPlusPlus

### A feature-rich, multi-tab desktop text editor built with C# and WPF — inspired by Notepad++.

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen?style=flat)](https://github.com/MNTN05/NotepadPlusPlus)
[![License](https://img.shields.io/badge/license-MIT-blue?style=flat)](LICENSE)
[![Version](https://img.shields.io/badge/version-1.0.0-informational?style=flat)](https://github.com/MNTN05/NotepadPlusPlus/releases)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Windows-0078D6?style=flat&logo=windows&logoColor=white)](https://github.com/MNTN05/NotepadPlusPlus)

</div>

---

## 📖 About the Project

**NotepadPlusPlus** is a Windows desktop text editor that replicates and extends the core experience of the popular Notepad++ application. It was built to demonstrate proficiency in **modern C# (.NET 8)**, **WPF UI development**, and enterprise-grade software architecture patterns including **MVVM**, **Dependency Injection**, and a **clean Service Layer**.

> **The problem it solves:** Developers and power users need a lightweight yet capable text editor that supports simultaneous file editing, powerful find/replace, and file-system navigation — all from a single, responsive desktop window.

**Why it was built:** This project serves as a practical exercise in building a non-trivial desktop application from scratch, applying the same clean-architecture principles used in production .NET systems — without relying on third-party UI component libraries.

---

## ✨ Key Features

- **Multi-tab editing** — Open and manage multiple files simultaneously in a familiar tab interface.
- **Full file management** — Create, open, save, and save-as text files with unsaved-change tracking (visual dirty-state indicator per tab).
- **Find & Replace** — Search with forward/backward navigation; replace a single occurrence or all occurrences across the active tab or all open tabs at once.
- **Text transformations** — Convert selection or entire document to uppercase/lowercase; remove all empty lines with a single action.
- **Go to Line** — Navigate directly to any line number via a dialog, mirroring professional editor UX.
- **Read-only mode** — Toggle write protection on any tab to prevent accidental edits.
- **Folder Explorer panel** — Browse a full directory tree in a side panel with lazy loading; open any file directly into a new tab.
- **Clipboard-aware folder operations** — Copy file paths to the clipboard; copy and paste entire folder structures from the context menu.
- **Keyboard shortcuts** — `Ctrl+N`, `Ctrl+O`, `Ctrl+S`, `Ctrl+Shift+S`, `Ctrl+F`, `Ctrl+H` and more for a familiar, productive workflow.
- **Interface-driven services** — `IFileService`, `IDialogService`, and `IClipboardService` enable clean separation and straightforward unit testability.

---

## 🛠 Tech Stack

### Desktop UI — **WPF (Windows Presentation Foundation)**
Pure WPF was chosen over third-party UI frameworks to keep the dependency footprint minimal and to demonstrate deep knowledge of the platform's data-binding engine, value converters, attached behaviors, and `DataTemplate`-driven UI composition.

### Language & Runtime — **C# 12 / .NET 8.0**
.NET 8 is the latest LTS release of the .NET platform, providing top-tier performance, modern language features (nullable reference types, pattern matching, records), and long-term support — the right choice for any serious Windows desktop application.

### MVVM Toolkit — **CommunityToolkit.Mvvm 8.2.2**
Microsoft's official Community Toolkit replaces boilerplate `INotifyPropertyChanged` implementations with source-generated properties (`[ObservableProperty]`) and commands (`[RelayCommand]`), keeping ViewModels lean and readable without introducing an opinionated framework overhead.

| Category | Technology | Rationale |
|---|---|---|
| **Language** | C# 12 | Modern syntax, strong typing, nullable analysis |
| **Runtime** | .NET 8.0 | LTS, high performance, cross-version compatibility |
| **UI Framework** | WPF / XAML | Native Windows rendering, rich data-binding |
| **MVVM Toolkit** | CommunityToolkit.Mvvm 8.2.2 | Source-generated commands & observables, minimal overhead |
| **Build Tool** | MSBuild / `dotnet CLI` | Standard .NET build pipeline |
| **Version Control** | Git / GitHub | Distributed VCS with PR-based workflow |

---

## 🏗 Architecture & Design

The project follows a strict **MVVM (Model-View-ViewModel)** architecture with a dedicated **Service Layer**, giving it the structure of a production-grade WPF application.

```
┌───────────────────────────────────────────────────────────┐
│                     View Layer (XAML)                     │
│      MainWindow · FindReplaceWindow · AboutWindow         │
└─────────────────────────┬─────────────────────────────────┘
                          │  Data Binding & Commands
┌─────────────────────────▼─────────────────────────────────┐
│                  ViewModel Layer (MVVM)                   │
│  MainViewModel ──┬── FileTabViewModel                     │
│                  ├── FindReplaceViewModel                 │
│                  └── FileSystemItemViewModel              │
└─────────────────────────┬─────────────────────────────────┘
                          │  Constructor Injection
┌─────────────────────────▼─────────────────────────────────┐
│                   Service Layer                           │
│  IFileService ←── FileService                             │
│  IDialogService ←── DialogService                        │
│  IClipboardService ←── ClipboardService                  │
└─────────────────────────┬─────────────────────────────────┘
                          │
┌─────────────────────────▼─────────────────────────────────┐
│              System / Framework Layer                     │
│           System.IO · Microsoft.Win32 · WPF               │
└───────────────────────────────────────────────────────────┘
```

**Design patterns applied:**

- **MVVM** — Total separation between UI and business logic; Views contain zero application logic.
- **Dependency Injection** — Services are manually wired in `App.xaml.cs` and injected via constructors, decoupling ViewModels from concrete implementations.
- **Service Layer** — All I/O, dialog, and clipboard operations are abstracted behind interfaces, enabling mocking and future testability.
- **Observer / Observable Collections** — `ObservableCollection<T>` drives automatic UI refresh for the tab strip and folder tree without manual binding updates.
- **Attached Behavior** — `TextBoxBehavior` implements an attached property to synchronise `SelectionStart`, `SelectionLength`, and `CaretIndex` with the ViewModel — a pattern required to bridge WPF control state that is not natively bindable.
- **Value Converters** — `BoolToColorConverter`, `BoolToVisibilityConverter`, and `InverseBoolToVisibilityConverter` keep all visual-state logic out of the ViewModel and in the presentation tier where it belongs.

---

## 🚀 Getting Started

### Prerequisites

- **Windows 10 / 11** (WPF is Windows-only)
- **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** (or later)
- **Visual Studio 2022** (recommended) _or_ the `dotnet` CLI

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
   dotnet build --configuration Release
   ```

### Environment Variables

This project does not require any environment variables or external configuration files. All settings are compile-time constants.

### Running the Application

**Via the .NET CLI:**

```bash
dotnet run --project NotepadPlusPlus.csproj
```

**Via Visual Studio 2022:**

Open `NotepadPlusPlus.sln`, set `NotepadPlusPlus` as the startup project, and press **F5** (Debug) or **Ctrl+F5** (Run without debugging).

The compiled executable can also be found at:

```
bin\Release\net8.0-windows\NotepadPlusPlus.exe
```

---

## 🖥 Usage

> _Screenshots and GIFs demonstrating the application will be added here._

| Feature | Preview |
|---|---|
| Multi-tab editor | _(screenshot)_ |
| Find & Replace dialog | _(screenshot)_ |
| Folder Explorer panel | _(screenshot)_ |
| Text transformations | _(screenshot)_ |

---

## 🧩 Challenges & Learnings

### Challenge: Synchronising WPF TextBox State with the ViewModel

**The problem:** WPF's `TextBox` control exposes `SelectionStart`, `SelectionLength`, and `CaretIndex` as standard CLR properties — not `DependencyProperty` instances — which means they cannot be used as binding targets in XAML. Implementing a proper MVVM architecture required that these values be accessible from the ViewModel (e.g., to position the caret after a Find/Replace operation), but there was no built-in way to bind them.

**The solution:** An **Attached Behavior** (`TextBoxBehavior`) was implemented using `DependencyProperty` registration. The behavior hooks into the `TextBox`'s `SelectionChanged` event to propagate UI state into the ViewModel, and conversely, the ViewModel can set the attached properties to drive the `TextBox` state back from a command. This pattern is a standard, MVVM-compliant alternative to code-behind event handlers and keeps the View-ViewModel boundary clean.

**Takeaway:** WPF's extensibility model — attached properties, attached behaviors, and value converters — can solve virtually any binding limitation without breaking architectural boundaries, but it requires a thorough understanding of the framework's `DependencyProperty` system.

---

## 🗺 Future Improvements

- [ ] **Syntax highlighting** — Integrate a syntax-highlighting engine (e.g., AvalonEdit) to support common programming languages.
- [ ] **Line numbers** — Display a line-number gutter alongside the editor area.
- [ ] **Settings & themes** — Persist user preferences (font size, colour theme, word wrap) to an application config file.
- [ ] **Status bar** — Show live cursor position (line/column), total line count, and encoding information.
- [ ] **Encoding support** — Allow opening and saving files in different encodings (UTF-8, UTF-16, ASCII).
- [ ] **Recent files list** — Track and display recently opened files in the File menu.
- [ ] **Unit & integration tests** — Add a test project using xUnit and Moq to cover service and ViewModel logic via the existing interface abstractions.
- [ ] **Auto-save & crash recovery** — Periodically back up unsaved content to a temp location to recover from unexpected crashes.

---

## 📬 Contact

**David-Gabriel Munteanu**

Applied Computer Science Student · Transilvania University of Brașov

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0A66C2?style=flat&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/david-gabriel-munteanu)
[![Email](https://img.shields.io/badge/Email-EA4335?style=flat&logo=gmail&logoColor=white)](mailto:david-gabriel.munteanu@student.unitbv.ro)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white)](https://github.com/MNTN05)

---

<div align="center">

_Built with ❤️ using C# · .NET 8 · WPF_

</div>
