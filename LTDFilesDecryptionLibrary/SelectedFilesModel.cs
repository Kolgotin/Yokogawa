namespace LTDFilesDecryptionLibrary
{
    public class SelectedFilesModel
    {
        public string FullPath { get; private set; }
        public string FileName { get; private set; }

        public SelectedFilesModel(string filePath, int sameDirIndex = 0)
        {
            FullPath = filePath;
            FileName = filePath.Substring(sameDirIndex);
        }
    }
}
