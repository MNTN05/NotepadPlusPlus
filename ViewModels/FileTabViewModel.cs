using System;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace NotepadPlusPlus.ViewModels
{
    public partial class FileTabViewModel : ObservableObject
    {
        private readonly int _newFileIndex;
        private bool _suppressModified;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Header))]
        [NotifyPropertyChangedFor(nameof(IsNew))]
        private string? _filePath;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Header))]
        private bool _isModified;

        [ObservableProperty]
        private string _content = string.Empty;

        [ObservableProperty]
        private bool _isReadOnly;

        [ObservableProperty]
        private int _selectionStart;

        [ObservableProperty]
        private int _selectionLength;

        [ObservableProperty]
        private int _caretIndex;

        public bool IsNew => FilePath == null;
        public string Header => IsNew ? $"File {_newFileIndex}" : System.IO.Path.GetFileName(FilePath)!;

        public FileTabViewModel(int newFileIndex)
        {
            _newFileIndex = newFileIndex;
        }

        public FileTabViewModel(string filePath, string content)
        {
            _suppressModified = true;
            _filePath = filePath;
            _content = content;
            _suppressModified = false;
        }

        partial void OnContentChanged(string value)
        {
            if (!_suppressModified)
                IsModified = true;
        }

        public void MarkAsSaved(string filePath)
        {
            _suppressModified = true;
            FilePath = filePath;
            IsModified = false;
            _suppressModified = false;
        }

        public void Highlight(int start, int length)
        {
            CaretIndex = start;
            SelectionStart = start;
            SelectionLength = length;
        }

        public void ConvertSelectionToUpper()
        {
            if (SelectionLength <= 0) return;
            ApplyToSelection(s => s.ToUpper());
        }

        public void ConvertSelectionToLower()
        {
            if (SelectionLength <= 0) return;
            ApplyToSelection(s => s.ToLower());
        }

        public void RemoveEmptyLines()
        {
            Content = Regex.Replace(Content, @"^\s*$\n?", string.Empty,
                RegexOptions.Multiline).TrimEnd('\r', '\n');
        }

        public void GoToLine(int lineNumber)
        {
            var lines = Content.Split('\n');
            if (lineNumber < 1 || lineNumber > lines.Length) return;
            int pos = 0;
            for (int i = 0; i < lineNumber - 1; i++)
                pos += lines[i].Length + 1;
            CaretIndex = pos;
        }

        [RelayCommand]
        private void ToggleReadOnly() => IsReadOnly = !IsReadOnly;

        private void ApplyToSelection(Func<string, string> transform)
        {
            var start = SelectionStart;
            var len = SelectionLength;
            var before = Content[..start];
            var selected = transform(Content.Substring(start, len));
            var after = Content[(start + len)..];
            _suppressModified = false;
            Content = before + selected + after;
            SelectionStart = start;
            SelectionLength = len;
        }
    }
}