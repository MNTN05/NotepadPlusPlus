namespace NotepadPlusPlus.Services.Interfaces
{
    public interface IClipboardService
    {
        void SetText(string text);
        string GetText();
        bool ContainsText();
        void SetFolderPath(string path);
        string GetFolderPath();
        bool ContainsFolderPath();
    }
}