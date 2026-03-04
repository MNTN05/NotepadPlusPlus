using System.Windows;
using NotepadPlusPlus.Services;
using NotepadPlusPlus.ViewModels;

namespace NotepadPlusPlus
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var fileService = new FileService();
            var dialogService = new DialogService();
            var clipboardService = new ClipboardService();

            var mainViewModel = new MainViewModel(fileService, dialogService, clipboardService);

            var mainWindow = new MainWindow { DataContext = mainViewModel };
            mainWindow.Show();
        }
    }
}