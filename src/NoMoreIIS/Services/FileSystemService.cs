using System;
using System.IO;

namespace NoMoreIIS.Services
{
    public class FileSystemService : IFileSystemService
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string GetFileContent(string file)
        {
            return File.ReadAllText(file);
        }

        public void WriteAllText(string file, string content)
        {
            File.WriteAllText(file, content);
        }
    }
}
