using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotepadPlusPlus.Services.Interfaces;
using NotepadPlusPlus.Views;

namespace NotepadPlusPlus.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IFileService _fileService;
        private readonly IDialogService _dialogService;
        private readonly IClipboardService _clipboardService;

        private int _newFileCounter = 1;
        private string _lastSearchText = string.Empty;
        private int _lastSearchPosition;

        [ObservableProperty]
        private ObservableCollection<FileTabViewModel> _tabs = new();

        [ObservableProperty]
        private FileTabViewModel _selectedTab;

        [ObservableProperty]
        private ObservableCollection<FileSystemItemViewModel> _rootItems = new();

        [ObservableProperty]
        private bool _isFolderExplorerVisible;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSelectedTab))]
        private bool _isAllTabs;

        [ObservableProperty]
        private FindReplaceViewModel _findReplaceViewModel;

        public bool IsSelectedTab
        {
            get => !IsAllTabs;
            set { if (value) IsAllTabs = false; }
        }

        public string StudentName => "Munteanu David-Gabriel";
        public string StudentGroup => "10LF342";
        public string StudentEmail => "david-gabriel.munteanu@student.unitbv.ro";

        public MainViewModel(IFileService fileService, IDialogService dialogService, IClipboardService clipboardService)
        {
            _fileService = fileService;
            _dialogService = dialogService;
            _clipboardService = clipboardService;

            _findReplaceViewModel = new FindReplaceViewModel
            {
                OnFind = InitiateFind,
                OnFindNext = () => FindNextOccurrence(forward: true),
                OnFindPrevious = () => FindNextOccurrence(forward: false),
                OnReplace = ReplaceCurrentOccurrence,
                OnReplaceAll = ReplaceAllOccurrences
            };

            NewFile();
        }

        [RelayCommand]
        private void NewFile()
        {
            var tab = new FileTabViewModel(_newFileCounter++);
            Tabs.Add(tab);
            SelectedTab = tab;
        }

        [RelayCommand]
        private void OpenFile()
        {
            var path = _dialogService.ShowOpenFileDialog(
                "Text Files (*.txt)|*.txt|All Files (*.*)|*.*", "txt");

            if (path is null) return;

            var existing = Tabs.FirstOrDefault(t => t.FilePath == path);
            if (existing is not null) { SelectedTab = existing; return; }

            try
            {
                var tab = new FileTabViewModel(path, _fileService.ReadFile(path));
                Tabs.Add(tab);
                SelectedTab = tab;
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessageDialog($"Could not open file:\n{ex.Message}", "Error");
            }
        }

        [RelayCommand]
        private void SaveFile()
        {
            if (SelectedTab is not null) SaveTab(SelectedTab);
        }

        [RelayCommand]
        private void SaveFileAs()
        {
            if (SelectedTab is not null) SaveTabAs(SelectedTab);
        }

        [RelayCommand]
        private void CloseTab(FileTabViewModel tab)
        {
            var target = tab ?? SelectedTab;
            if (target is null) return;

            if (target.IsModified)
            {
                var result = _dialogService.ShowConfirmationDialog(
                    $"'{target.Header}' has unsaved changes. Save before closing?",
                    "Unsaved Changes");

                if (result == ConfirmDialogResult.Cancel) return;
                if (result == ConfirmDialogResult.Yes && !SaveTab(target)) return;
            }

            var index = Tabs.IndexOf(target);
            Tabs.Remove(target);

            if (Tabs.Count > 0)
                SelectedTab = Tabs[Math.Max(0, index - 1)];
        }

        [RelayCommand]
        private void CloseAllFiles()
        {
            foreach (var tab in Tabs.ToList())
            {
                if (tab.IsModified)
                {
                    SelectedTab = tab;
                    var result = _dialogService.ShowConfirmationDialog(
                        $"'{tab.Header}' has unsaved changes. Save before closing?",
                        "Unsaved Changes");

                    if (result == ConfirmDialogResult.Cancel) return;
                    if (result == ConfirmDialogResult.Yes && !SaveTab(tab)) return;
                }
                Tabs.Remove(tab);
            }
        }

        [RelayCommand]
        private void ExitApplication() => Application.Current.MainWindow?.Close();

        [RelayCommand]
        private void OpenFolderExplorer()
        {
            var path = _dialogService.ShowFolderBrowserDialog();
            if (path is null) return;

            RootItems.Clear();
            var root = new FileSystemItemViewModel(
                Path.GetFileName(path), path, true, _fileService, _clipboardService)
            {
                RequestOpenFile = OpenFileFromPath
            };
            root.LoadChildren();
            RootItems.Add(root);
            IsFolderExplorerVisible = true;
        }

        [RelayCommand]
        private void SetStandardView() => IsFolderExplorerVisible = false;

        [RelayCommand]
        private void SetFolderExplorerView()
        {
            if (RootItems.Count == 0) OpenFolderExplorer();
            else IsFolderExplorerVisible = true;
        }

        [RelayCommand]
        private void ShowFind()
        {
            FindReplaceViewModel.Mode = FindReplaceMode.Find;
            OpenFindReplaceWindow();
        }

        [RelayCommand]
        private void ShowReplace()
        {
            FindReplaceViewModel.Mode = FindReplaceMode.Replace;
            OpenFindReplaceWindow();
        }

        [RelayCommand]
        private void ShowAbout()
        {
            var win = new AboutWindow { DataContext = this, Owner = Application.Current.MainWindow };
            win.ShowDialog();
        }

        [RelayCommand]
        private void ConvertToUpper() => SelectedTab?.ConvertSelectionToUpper();

        [RelayCommand]
        private void ConvertToLower() => SelectedTab?.ConvertSelectionToLower();

        [RelayCommand]
        private void RemoveEmptyLines() => SelectedTab?.RemoveEmptyLines();

        [RelayCommand]
        private void GoToLine()
        {
            var input = _dialogService.ShowInputDialog("Enter line number:", "Go To Line", "1");
            if (input is not null && int.TryParse(input, out int line))
                SelectedTab?.GoToLine(line);
        }

        [RelayCommand]
        private void ToggleReadOnly()
        {
            if (SelectedTab is not null) SelectedTab.IsReadOnly = !SelectedTab.IsReadOnly;
        }

        public bool RequestExit()
        {
            foreach (var tab in Tabs.ToList())
            {
                if (!tab.IsModified) continue;
                SelectedTab = tab;
                var result = _dialogService.ShowConfirmationDialog(
                    $"'{tab.Header}' has unsaved changes. Save before exiting?",
                    "Unsaved Changes");

                if (result == ConfirmDialogResult.Cancel) return false;
                if (result == ConfirmDialogResult.Yes && !SaveTab(tab)) return false;
            }
            return true;
        }

        public void OpenFileFromPath(string path)
        {
            var existing = Tabs.FirstOrDefault(t => t.FilePath == path);
            if (existing is not null) { SelectedTab = existing; return; }

            try
            {
                var tab = new FileTabViewModel(path, _fileService.ReadFile(path));
                Tabs.Add(tab);
                SelectedTab = tab;
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessageDialog($"Could not open file:\n{ex.Message}", "Error");
            }
        }

        private bool SaveTab(FileTabViewModel tab)
        {
            if (tab.IsNew) return SaveTabAs(tab);
            try
            {
                _fileService.WriteFile(tab.FilePath, tab.Content);
                tab.MarkAsSaved(tab.FilePath);
                return true;
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessageDialog($"Could not save file:\n{ex.Message}", "Error");
                return false;
            }
        }

        private bool SaveTabAs(FileTabViewModel tab)
        {
            var path = _dialogService.ShowSaveFileDialog(
                tab.Header, "Text Files (*.txt)|*.txt|All Files (*.*)|*.*", "txt");

            if (path is null) return false;

            try
            {
                _fileService.WriteFile(path, tab.Content);
                tab.MarkAsSaved(path);
                return true;
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessageDialog($"Could not save file:\n{ex.Message}", "Error");
                return false;
            }
        }

        private void InitiateFind(string searchText)
        {
            _lastSearchText = searchText;
            _lastSearchPosition = 0;
            FindReplaceViewModel.StatusMessage = string.Empty;
            FindNextOccurrence(forward: true);
        }

        private void FindNextOccurrence(bool forward)
        {
            if (string.IsNullOrEmpty(_lastSearchText)) return;

            var targets = IsAllTabs ? Tabs.ToList() : new List<FileTabViewModel> { SelectedTab };
            targets.RemoveAll(t => t is null);
            if (targets.Count == 0) return;

            int startTabIndex = targets.IndexOf(SelectedTab);
            if (startTabIndex < 0) startTabIndex = 0;

            if (forward)
            {
                for (int t = 0; t < targets.Count; t++)
                {
                    var tab = targets[(startTabIndex + t) % targets.Count];
                    int searchFrom = (t == 0) ? _lastSearchPosition : 0;

                    int idx = tab.Content.IndexOf(_lastSearchText, searchFrom, StringComparison.OrdinalIgnoreCase);

                    if (idx < 0 && t == 0 && searchFrom > 0)
                        idx = tab.Content.IndexOf(_lastSearchText, 0, StringComparison.OrdinalIgnoreCase);

                    if (idx >= 0)
                    {
                        SelectedTab = tab;
                        tab.Highlight(idx, _lastSearchText.Length);
                        _lastSearchPosition = idx + _lastSearchText.Length;
                        FindReplaceViewModel.StatusMessage = $"Match found at position {idx}.";
                        return;
                    }
                }
            }
            else
            {
                for (int t = 0; t < targets.Count; t++)
                {
                    int tabIndex = (startTabIndex - t + targets.Count) % targets.Count;
                    var tab = targets[tabIndex];

                    int searchTo = (t == 0 && _lastSearchPosition > 0)
                        ? _lastSearchPosition - 1
                        : tab.Content.Length;

                    if (searchTo <= 0) continue;

                    int clampedTo = Math.Min(searchTo - 1, tab.Content.Length - 1);
                    if (clampedTo < 0) continue;

                    int idx = tab.Content.LastIndexOf(_lastSearchText, clampedTo, StringComparison.OrdinalIgnoreCase);

                    if (idx >= 0)
                    {
                        SelectedTab = tab;
                        tab.Highlight(idx, _lastSearchText.Length);
                        _lastSearchPosition = idx;
                        FindReplaceViewModel.StatusMessage = $"Match found at position {idx}.";
                        return;
                    }
                }
            }

            FindReplaceViewModel.StatusMessage = "No match found.";
        }

        private void ReplaceCurrentOccurrence(string searchText, string replaceText)
        {
            if (SelectedTab is null) return;
            _lastSearchText = searchText;

            var tab = SelectedTab;
            int searchFrom = Math.Max(0, tab.SelectionStart);
            int idx = tab.Content.IndexOf(searchText, searchFrom, StringComparison.OrdinalIgnoreCase);

            if (idx < 0)
                idx = tab.Content.IndexOf(searchText, 0, StringComparison.OrdinalIgnoreCase);

            if (idx < 0)
            {
                FindReplaceViewModel.StatusMessage = "No match found.";
                return;
            }

            tab.Content = tab.Content[..idx] + replaceText + tab.Content[(idx + searchText.Length)..];
            _lastSearchPosition = idx + replaceText.Length;
            tab.Highlight(idx, replaceText.Length);
            FindReplaceViewModel.StatusMessage = "Replaced 1 occurrence.";
        }

        private void ReplaceAllOccurrences(string searchText, string replaceText)
        {
            var targets = IsAllTabs ? Tabs.ToList() : new List<FileTabViewModel> { SelectedTab };
            int count = 0;

            foreach (var tab in targets)
            {
                if (tab is null) continue;
                var original = tab.Content;
                var updated = original.Replace(searchText, replaceText, StringComparison.OrdinalIgnoreCase);
                if (original != updated) { tab.Content = updated; count++; }
            }

            _lastSearchPosition = 0;
            FindReplaceViewModel.StatusMessage = count > 0
                ? $"Replaced in {count} file(s)."
                : "No matches found.";
        }

        private void OpenFindReplaceWindow()
        {
            var existing = Application.Current.Windows.OfType<FindReplaceWindow>().FirstOrDefault();
            if (existing is not null) { existing.Activate(); return; }

            var win = new FindReplaceWindow
            {
                DataContext = FindReplaceViewModel,
                Owner = Application.Current.MainWindow
            };
            win.Show();
        }
    }
}