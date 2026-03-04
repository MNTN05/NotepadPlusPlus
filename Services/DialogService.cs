using System.Windows;
using Microsoft.Win32;
using NotepadPlusPlus.Services.Interfaces;

namespace NotepadPlusPlus.Services
{
    public class DialogService : IDialogService
    {
        public string ShowOpenFileDialog(string filter, string defaultExtension)
        {
            var dialog = new OpenFileDialog { Filter = filter, DefaultExt = defaultExtension };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string ShowSaveFileDialog(string initialFileName, string filter, string defaultExtension)
        {
            var dialog = new SaveFileDialog
            {
                FileName = initialFileName,
                Filter = filter,
                DefaultExt = defaultExtension
            };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public ConfirmDialogResult ShowConfirmationDialog(string message, string title)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            return result switch
            {
                MessageBoxResult.Yes => ConfirmDialogResult.Yes,
                MessageBoxResult.No => ConfirmDialogResult.No,
                _ => ConfirmDialogResult.Cancel
            };
        }

        public void ShowMessageDialog(string message, string title) =>
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

        public string ShowFolderBrowserDialog()
        {
            var dialog = new OpenFolderDialog { Title = "Select Folder" };
            return dialog.ShowDialog() == true ? dialog.FolderName : null;
        }

        public string ShowInputDialog(string prompt, string title, string defaultValue = "")
        {
            var win = new Window
            {
                Title = title,
                Width = 320,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow,
                ResizeMode = ResizeMode.NoResize
            };

            var panel = new System.Windows.Controls.StackPanel { Margin = new Thickness(12) };
            var label = new System.Windows.Controls.TextBlock { Text = prompt, Margin = new Thickness(0, 0, 0, 6) };
            var textBox = new System.Windows.Controls.TextBox { Text = defaultValue };
            var btnRow = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var ok = new System.Windows.Controls.Button { Content = "OK", Width = 75, IsDefault = true, Margin = new Thickness(0, 0, 6, 0) };
            var cancel = new System.Windows.Controls.Button { Content = "Cancel", Width = 75, IsCancel = true };

            string result = null;
            ok.Click += (_, _) => { result = textBox.Text; win.DialogResult = true; };
            cancel.Click += (_, _) => win.DialogResult = false;

            btnRow.Children.Add(ok);
            btnRow.Children.Add(cancel);
            panel.Children.Add(label);
            panel.Children.Add(textBox);
            panel.Children.Add(btnRow);
            win.Content = panel;

            return win.ShowDialog() == true ? result : null;
        }
    }
}