using System;
using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotepadPlusPlus.Services.Interfaces;

namespace NotepadPlusPlus.ViewModels
{
    public partial class FileSystemItemViewModel : ObservableObject
    {
        private readonly IFileService _fileService;
        private readonly IClipboardService _clipboardService;

        [ObservableProperty]
        private bool _isExpanded;

        public string Name { get; }
        public string FullPath { get; }
        public bool IsDirectory { get; }
        public ObservableCollection<FileSystemItemViewModel> Children { get; } = new();

        public Action<string> RequestOpenFile { get; set; }

        public FileSystemItemViewModel(
            string name, string fullPath, bool isDirectory,
            IFileService fileService, IClipboardService clipboardService)
        {
            Name = name;
            FullPath = fullPath;
            IsDirectory = isDirectory;
            _fileService = fileService;
            _clipboardService = clipboardService;

            if (isDirectory && !string.IsNullOrEmpty(fullPath))
                Children.Add(CreatePlaceholder());
        }

        partial void OnIsExpandedChanged(bool value)
        {
            if (value && IsDirectory && !string.IsNullOrEmpty(FullPath))
                LoadChildren();
        }

        public void LoadChildren()
        {
            Children.Clear();
            try
            {
                foreach (var dir in _fileService.GetDirectories(FullPath))
                    Children.Add(CreateChild(Path.GetFileName(dir), dir, true));

                foreach (var file in _fileService.GetFiles(FullPath))
                    Children.Add(CreateChild(Path.GetFileName(file), file, false));
            }
            catch { }
        }

        [RelayCommand]
        private void NewFile()
        {
            string name = "newfile.txt";
            string path = _fileService.CombinePath(FullPath, name);
            int i = 1;
            while (_fileService.FileExists(path))
            {
                name = $"newfile{i++}.txt";
                path = _fileService.CombinePath(FullPath, name);
            }
            _fileService.CreateFile(path);
            LoadChildren();
        }

        [RelayCommand]
        private void CopyPath() => _clipboardService.SetText(FullPath);

        [RelayCommand]
        private void CopyFolder()
        {
            if (IsDirectory)
                _clipboardService.SetFolderPath(FullPath);
        }

        [RelayCommand]
        private void PasteFolder()
        {
            if (!IsDirectory) return;
            var source = _clipboardService.GetFolderPath();
            if (string.IsNullOrEmpty(source)) return;

            var dest = _fileService.CombinePath(FullPath, Path.GetFileName(source));
            _fileService.CopyDirectory(source, dest);
            LoadChildren();
        }

        private FileSystemItemViewModel CreateChild(string name, string path, bool isDir) =>
            new(name, path, isDir, _fileService, _clipboardService) { RequestOpenFile = RequestOpenFile };

        private FileSystemItemViewModel CreatePlaceholder() =>
            new("...", string.Empty, false, _fileService, _clipboardService);
    }
}