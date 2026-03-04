using System.Collections.Generic;
using System.IO;
using NotepadPlusPlus.Services.Interfaces;

namespace NotepadPlusPlus.Services
{
    public class FileService : IFileService
    {
        public string ReadFile(string path) => File.ReadAllText(path);

        public void WriteFile(string path, string content) => File.WriteAllText(path, content);

        public void CreateFile(string path)
        {
            using var stream = File.Create(path);
        }

        public void CopyDirectory(string sourcePath, string destinationPath)
        {
            Directory.CreateDirectory(destinationPath);
            var dir = new DirectoryInfo(sourcePath);

            foreach (var file in dir.GetFiles())
                file.CopyTo(Path.Combine(destinationPath, file.Name), true);

            foreach (var subDir in dir.GetDirectories())
                CopyDirectory(subDir.FullName, Path.Combine(destinationPath, subDir.Name));
        }

        public bool FileExists(string path) => File.Exists(path);

        public IEnumerable<string> GetFiles(string path) => Directory.GetFiles(path);

        public IEnumerable<string> GetDirectories(string path) => Directory.GetDirectories(path);

        public string CombinePath(string path1, string path2) => Path.Combine(path1, path2);
    }
}