namespace NotepadPlusPlus.Services.Interfaces
{
    public enum ConfirmDialogResult { Yes, No, Cancel }

    public interface IDialogService
    {
        string ShowOpenFileDialog(string filter, string defaultExtension);
        string ShowSaveFileDialog(string initialFileName, string filter, string defaultExtension);
        ConfirmDialogResult ShowConfirmationDialog(string message, string title);
        void ShowMessageDialog(string message, string title);
        string ShowFolderBrowserDialog();
        string ShowInputDialog(string prompt, string title, string defaultValue = "");
    }
}