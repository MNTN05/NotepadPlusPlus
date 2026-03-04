using System.Collections.Generic;

namespace NotepadPlusPlus.Services.Interfaces
{
    public interface IFileService
    {
        string ReadFile(string path);
        void WriteFile(string path, string content);
        void CreateFile(string path);
        void CopyDirectory(string sourcePath, string destinationPath);
        bool FileExists(string path);
        IEnumerable<string> GetFiles(string path);
        IEnumerable<string> GetDirectories(string path);
        string CombinePath(string path1, string path2);
    }
}