using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using FileCollectorLibrary;

namespace CollectorFilesService
{
    public static class LogMessages
    {
        private static object locker = new object();
        public static string LogFilePath;        

        public static void Log(string str)
        {
            if (LogFilePath != null)
            {
                FileInfo fi = new FileInfo(LogFilePath);
                if (fi.Exists && fi.CreationTime.AddDays(1) <= DateTime.Now.Date)
                {
                    fi.CreationTime = DateTime.Now;
                    fi.Delete();
                }
                lock (locker)
                {
                    using (StreamWriter sw = new StreamWriter(LogFilePath, true))
                    {
                        sw.WriteLine(DateTime.Now + ": " + str);
                    }
                }
            }
        }
    }
}
