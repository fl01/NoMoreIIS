namespace NoMoreIIS.Services
{
    public interface IFileSystemService
    {
        bool FileExists(string path);

        string GetFileContent(string file);

        void WriteAllText(string file, string content);
    }
}
