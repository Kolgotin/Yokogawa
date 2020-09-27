using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

namespace FileCollectorLibrary
{
    /// <summary>
    /// принимает настройки и список новых файлов, перемещает из папки-источника в папку назначения
    /// </summary>
    public class MoveFilesList
    {
        string SourcePath;
        string DestPath;
        internal List<FilePathDate> NewFilesPaths = new List<FilePathDate>();
        DateTime NewDateTimeCopy;

        /// <summary>
        /// Интервал между копированием файлов в секундах
        /// </summary>
        public int Interval = 1;

        public MoveFilesList(Settings setting, List<FilePathDate> newFilesPaths)
        {            
            Constructor(setting.SourcePath, setting.BufferPath, newFilesPaths);
        }

        public MoveFilesList(string sourcePath, string destPath, List<FilePathDate> newFilesPaths)
        {
            Constructor(sourcePath, destPath, newFilesPaths);
        }

        private void Constructor(string sourcePath, string destPath, List<FilePathDate> newFilesPaths)
        {
            SourcePath = sourcePath;
            DestPath = destPath;
            foreach (FilePathDate file in newFilesPaths.OrderBy(x=> x.LastChangeDateTime))
            {
                NewFilesPaths.Add(new FilePathDate()
                {
                    Path = file.Path,
                    LastChangeDateTime = file.LastChangeDateTime
                });
            }
        }

        public void MoveFiles()
        {
            try
            {
                while (NewFilesPaths.Count > 0)
                {
                    string newFilePath = NewFilesPaths[0].Path;

                    if (MoveFile(newFilePath))
                    {
                        NewDateTimeCopy = NewFilesPaths[0].LastChangeDateTime;
                        NewFilesPaths.RemoveAt(0);
                        if (Interval > 0)
                        {
                            Thread.Sleep(Interval * 1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageShowMethod.ShowMethod(ex.Message);
                MessageShowMethod.ShowMethod("Прерывание метода MoveFiles");
            }
        }

        public DateTime StopMoveFiles()
        {

            NewFilesPaths.Clear();
            return NewDateTimeCopy;
        }

        private bool MoveFile(string currentFilePath)
        {
            try
            {
                if (currentFilePath.StartsWith(SourcePath))
                {
                    string sourceFileName = currentFilePath;
                    string destFileName = currentFilePath.Replace(SourcePath, DestPath);
                    var newDirectoryPath = Path.GetDirectoryName(destFileName);
                    Directory.CreateDirectory(newDirectoryPath);
                    File.Copy(sourceFileName, destFileName, true);

                    MessageShowMethod.ShowMethod("Скопирован файл " + currentFilePath);
                    return true;
                }
                else
                {
                    MessageShowMethod.ShowMethod("Путь источнику был изменён, не удалось скопировать файл: " + currentFilePath);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageShowMethod.ShowMethod(ex.Message);
                MessageShowMethod.ShowMethod("Ошибка в работе метода MoveFile при копировании файла " + currentFilePath);
                return false;
            }
        }
    }
}
