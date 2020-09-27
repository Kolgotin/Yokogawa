using System.IO;

namespace FileCollectorLibrary
{
    public class Settings
    {
        public string SourcePath
        {
            get;
            private set;
        }
        public string BufferPath
        {
            get;
            private set;
        }
        public string LastCopyDateTimeFilePath
        {
            get;
            private set;
        }
        public string ServiceName
        {
            get;
            private set;
        }
        public Settings(string sourcePath, string bufferPath)
        {
            Constructor(sourcePath, bufferPath);
        }

        public Settings(string sourcePath, string bufferPath, string serviceName)
        {
            Constructor(sourcePath, bufferPath);
            ServiceName = serviceName;
        }

        private void Constructor(string sourcePath, string bufferPath)
        {
            SourcePath = sourcePath;
            Directory.CreateDirectory(bufferPath);
            BufferPath = bufferPath;
            LastCopyDateTimeFilePath = Path.Combine(BufferPath, "LastCopyDateTime.xml");
        }
    }
}
