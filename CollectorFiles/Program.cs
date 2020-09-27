using FileCollectorLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CollectorFiles
{
    /// <summary>
    /// консольное приложение имитирующее работу службы копирования файлов, используется для тестов
    /// </summary>
    class Program
    {
        static string TargetFolderPath = @"C:\Users\nikok\Desktop\trend\";
        static GetSettings Settings = new GetSettings(TargetFolderPath);

        static string LogFilePath = Path.Combine(TargetFolderPath, "LogMessagesConsole.txt");
        static DateTime LastCopyDateTime;
        static DateTime NewDateTimeCopy;
        static MoveFilesList MoveFilesList;
        static DateToXml DateTimeSaveFile;
        static Thread CopyThread;
        static Timer MainTimer;

        static void Main(string[] args)
        {
            MessageShowMethod.ShowMethod = LogMessages;
            Settings.ReadSettingsFile();
            Console.WriteLine("Служба запущена.");
            if (Settings.IsActual)
            {
                MainTimer = new Timer(StartCopy, null, 0, 100000);
            }

            Console.WriteLine("Нажмите любую клавишу.");
            Console.ReadKey();
            OnStop();
        }

        static private void StartCopy(object obj)
        {            
            Console.WriteLine("Метод StartCopy запущен.");
            CopyThread = new Thread(new ThreadStart(Coping));
            CopyThread.Start();
            Thread.Sleep(70000);
            Console.WriteLine("Метод StartCopy завершен.");
            StopCopy();
        }

        static private void Coping()
        {
            Console.WriteLine("Копирование запущенно.");
            DateTimeSaveFile = new DateToXml(Settings.Settings[0].LastCopyDateTimeFilePath);
            LastCopyDateTime = DateTimeSaveFile.GetDataFromFile();

            NewFilesSearch newFilesSearch = new NewFilesSearch();
            List<FilePathDate> newFiles = newFilesSearch.GetFilesFromFolder(Settings.Settings[0].SourcePath, LastCopyDateTime);

            MoveFilesList = new MoveFilesList(Settings.Settings[0], newFiles)
            {
                Interval = 1
            };
            MoveFilesList?.MoveFiles();

            Console.WriteLine("Копирование файлов выполнено.");
        }

        static void Test(object obj)
        {
            for(int i = 0; i < 10; i++)
            {
                Console.WriteLine(i+ ":\t" +DateTime.Now);
                Thread.Sleep(500);
            }
        }

        static private void StopCopy()
        {
            Console.WriteLine("Метод StopCopy запущен.");
            if (CopyThread.IsAlive)
            {
                CopyThread.Suspend();
            }
            NewDateTimeCopy = MoveFilesList.StopMoveFiles();

            //сохраняем startDateCopy для следующего включения LastDateWrited
            DateTimeSaveFile.SetDateToFile(NewDateTimeCopy);

            Console.WriteLine("Метод StopCopy завершён.");
        }
        static void OnStop()
        {
            Console.WriteLine("Метод OnStop запущен.");
            StopCopy();
            MainTimer.Dispose();
            Console.WriteLine("Служба остановлена.");
        }

        static void LogMessages(string str)
        {
            FileInfo fi = new FileInfo(LogFilePath);
            if (fi.Exists && fi.CreationTime.AddDays(1) <= DateTime.Now.Date)
            {
                fi.CreationTime = DateTime.Now;
                fi.Delete();
            }
            Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
            using (StreamWriter sw = new StreamWriter(LogFilePath, true))
            {
                sw.WriteLine(DateTime.Now + ": " + str);
            }
        }
    }
}
