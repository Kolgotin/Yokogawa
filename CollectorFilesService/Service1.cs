using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.IO;
using System.Threading;
using FileCollectorLibrary;

namespace CollectorFilesService
{
    /// <summary>
    /// служба копирования файлов.
    /// настройки копирования должны находиться в папке с программой или в папке, путь к которой укзаан во входном параметре args
    /// </summary>
    public partial class Service1 : ServiceBase
    {
        /// <summary>
        /// Переменная хранит текущие настройки: частота копирования и список путей:
        /// путь к ресурсу, путь к буферу, название сервиса
        /// </summary>
        GetSettings Settings;

        /// <summary>
        /// список потоков - по одному для каждого элемента списка настроек копирования
        /// </summary>
        List<Thread> CopyThread = new List<Thread>();

        /// <summary>
        /// список элементов, каждый представляет собой пару: 
        /// список файлов для копирования формата MoveFilesList, 
        /// файл в который будет сохранена дата последнего скопированного файла - формат DateToXml
        /// </summary>
        List<ThreadsPack> MflDtx = new List<ThreadsPack>();

        /// <summary>
        /// таймер, в котором запускается основной метод программы, в него передаётся время цикла из настроек
        /// таймер прерывается при остановке службы
        /// </summary>
        Timer MainTimer;      

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string currentPath;
            if (args.Length > 0)
            {
                currentPath = args[0];
            }
            else
            { 
                currentPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            }
            LogMessages.LogFilePath = Path.Combine(currentPath, "LogMessages.txt");
            LogMessages.Log("Служба запущена.");

            MessageShowMethod.ShowMethod = LogMessages.Log;
            Settings = new GetSettings(currentPath);
            Settings.ReadSettingsFile();

            if (Settings.IsActual)
            {
                LogMessages.Log("Интервал всего цикла в настройках: " + Settings.IntervalAllCile);
                MainTimer = new Timer(StartCopy, null, 0, Settings.IntervalAllCile * 1000);
            }
            else
            {
                LogMessages.Log("Настройки не приняты - запущена остановка службы.");
                this.Stop();
            }
        }

        private void StartCopy(object obj)
        {
            LogMessages.Log("Цикл запущен.");
            int settingsCount = Settings.Settings.Count;
            LogMessages.Log("Всего " + settingsCount + " настроек");
            foreach (Settings set in Settings.Settings)
            {
                Thread currentThread = new Thread(new ParameterizedThreadStart(CopingFilesToBuffer));
                CopyThread.Add(currentThread);
                currentThread.Start(set);
                LogMessages.Log("поток " + set.ServiceName + " запущен.");
            }
            Thread.Sleep(Settings.WorkingCycle * 1000);
            LogMessages.Log("Цикл завершен.");
            StopCopy();
        }

        private void CopingFilesToBuffer(object obj)
        {
            Settings set = obj as Settings;
            LogMessages.Log("Копирование запущенно.");
            DateToXml dtx = new DateToXml(set.LastCopyDateTimeFilePath);
            DateTime lastCopyDateTime = dtx.GetDataFromFile();

            NewFilesSearch newFilesSearch = new NewFilesSearch();
            List<FilePathDate> newFiles = newFilesSearch.GetFilesFromFolder(set.SourcePath, lastCopyDateTime);

            MoveFilesList moveFilesList = new MoveFilesList(set, newFiles);
            
            MflDtx.Add(new ThreadsPack(moveFilesList,dtx));
            moveFilesList.MoveFiles();

            LogMessages.Log("Копирование файлов выполнено.");
        }

        protected override void OnStop()
        {
            try
            {
                LogMessages.Log("Остановка службы запущена.");
                StopCopy();
                if (MainTimer != null)
                {
                    MainTimer.Dispose();
                }
                LogMessages.Log("Служба остановлена.");
            }
            catch (Exception ex)
            {
                LogMessages.Log("Ошибка при выполнении остановки службы.");
            }
        }

        private void StopCopy()
        {
            LogMessages.Log("Остановка цикла запущена.");
            
            foreach(var thread in CopyThread)
            {
                if (thread.IsAlive)
                {
                    thread.Abort();
                }
            }
            CopyThread.Clear();

            MflDtx.ForEach(x =>
            {
                DateTime newDateTimeCopy = x.MoveFiles.StopMoveFiles();
                x.DateXml.SetDateToFile(newDateTimeCopy);
            });
            MflDtx.Clear();

            LogMessages.Log("Остановка завершена.");
        }
    }
}
