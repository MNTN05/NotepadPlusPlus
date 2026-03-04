using System.Windows;
using NotepadPlusPlus.Services.Interfaces;

namespace NotepadPlusPlus.Services
{
    public class ClipboardService : IClipboardService
    {
        private const string FolderPathKey = "NotepadPlusPlus_FolderPath";

        public void SetText(string text) => Clipboard.SetText(text);

        public string GetText() => Clipboard.ContainsText() ? Clipboard.GetText() : string.Empty;

        public bool ContainsText() => Clipboard.ContainsText();

        public void SetFolderPath(string path)
        {
            var data = new DataObject();
            data.SetData(FolderPathKey, path);
            Clipboard.SetDataObject(data, true);
        }

        public string GetFolderPath() => Clipboard.GetDataObject()?.GetData(FolderPathKey) as string;

        public bool ContainsFolderPath() => Clipboard.GetDataObject()?.GetDataPresent(FolderPathKey) == true;
    }
}