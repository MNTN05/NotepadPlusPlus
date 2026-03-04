using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace NotepadPlusPlus.ViewModels
{
    public enum FindReplaceMode { Find, Replace }

    public partial class FindReplaceViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _replaceText = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsReplaceMode))]
        private FindReplaceMode _mode = FindReplaceMode.Find;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        public bool IsReplaceMode => Mode == FindReplaceMode.Replace;

        public Action<string> OnFind { get; set; }
        public Action OnFindNext { get; set; }
        public Action OnFindPrevious { get; set; }
        public Action<string, string> OnReplace { get; set; }
        public Action<string, string> OnReplaceAll { get; set; }

        [RelayCommand]
        private void Find()
        {
            if (!string.IsNullOrEmpty(SearchText))
                OnFind?.Invoke(SearchText);
        }

        [RelayCommand]
        private void FindNext() => OnFindNext?.Invoke();

        [RelayCommand]
        private void FindPrevious() => OnFindPrevious?.Invoke();

        [RelayCommand]
        private void Replace()
        {
            if (!string.IsNullOrEmpty(SearchText))
                OnReplace?.Invoke(SearchText, ReplaceText);
        }

        [RelayCommand]
        private void ReplaceAll()
        {
            if (!string.IsNullOrEmpty(SearchText))
                OnReplaceAll?.Invoke(SearchText, ReplaceText);
        }
    }
}