using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NotepadPlusPlus.ViewModels;

namespace NotepadPlusPlus
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is MainViewModel vm && !vm.RequestExit())
                e.Cancel = true;
        }

        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not MainViewModel vm) return;
            if (sender is not TreeView tv) return;
            if (tv.SelectedItem is not FileSystemItemViewModel item) return;
            if (!item.IsDirectory)
                vm.OpenFileFromPath(item.FullPath);
        }
    }
}