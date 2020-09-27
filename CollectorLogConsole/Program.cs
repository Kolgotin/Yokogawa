using System;
using LogCollectorLibrary;

namespace CollectorLogConsole
{
    /// <summary>
    /// запускает LogReader - библиотеку
    /// в первый параметр передать путь к папке - ресурсу
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //string source = @"\\desktop5\STORAGE\TREND";
            string source = args[0];
            MessageShowMethod.ShowMethod = Console.WriteLine;
            if (string.IsNullOrEmpty(source))
            {
                MessageShowMethod.ShowMethod("задайте путь к папке-источнику");
            }
            else
            {
                LogReader logReader = new LogReader(source);
                logReader.ReadFiles();
            }
        }
    }
}
