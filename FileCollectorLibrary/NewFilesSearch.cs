using System;
using System.Collections.Generic;
using System.IO;

namespace FileCollectorLibrary
{
    public class NewFilesSearch
    {
        private List<FilePathDate> AllLTDFilesPathsInFolders = new List<FilePathDate>();

        public NewFilesSearch()
        {
        }
        public List<FilePathDate> GetFilesFromFolder(string sourcePath, DateTime lastDateWrited)
        {
            try
            {
                string[] patterns = { "*.LTD*", "*.LOG" };
                var currentLTDFiles = MySearch(sourcePath, patterns);
                foreach (string currentFilePath in currentLTDFiles)
                {
                    DateTime lastWriteTime = File.GetLastWriteTime(currentFilePath);
                    if (lastWriteTime >= lastDateWrited)
                    {
                        AllLTDFilesPathsInFolders.Add(new FilePathDate()
                        {
                            Path = currentFilePath,
                            LastChangeDateTime = lastWriteTime
                        });
                    }
                }

                MessageShowMethod.ShowMethod("Обнаружено " + AllLTDFilesPathsInFolders.Count + " новых файлов");
            }
            catch(Exception ex)
            {
                MessageShowMethod.ShowMethod(ex.Message);
                MessageShowMethod.ShowMethod("Ошибка в методе GetFilesFromFolder");
            }
            return AllLTDFilesPathsInFolders;
        }

        private List<string> MySearch(string sourcePath, string[] patterns)
        {
            List<string> fileList = new List<string>();
            foreach (var p in patterns)
            {
                fileList.AddRange(Directory.GetFiles(sourcePath, p, SearchOption.AllDirectories));
            }
            return fileList;
        }

        ~NewFilesSearch()
        {
            AllLTDFilesPathsInFolders.Clear();
        }
    }
}
