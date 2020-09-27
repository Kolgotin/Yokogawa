using System;
using System.Collections.Generic;
using System.IO;

namespace FileCollectorLibrary
{
    /// <summary>
    /// Считывает файл с настройками - текстовый файл Settings.txt в корневой папке
    /// настройки в файле должны быть записаны в порядке: первая строка - 
    /// путь к ресурсу, путь к буферу, название сервиса, через символ |
    /// </summary>
    public class GetSettings
    {
        const int MIN_INTERVAL_ALL_CICLE = 600;
        const int SAVING_TIME = 60;
        private int _IntervalAllCile;
        public int IntervalAllCile => _IntervalAllCile > MIN_INTERVAL_ALL_CICLE ? _IntervalAllCile : MIN_INTERVAL_ALL_CICLE;
        public int WorkingCycle => IntervalAllCile - MIN_INTERVAL_ALL_CICLE;

        enum SettingsLineParts
        {
            SourcePathPart = 0,
            BufferPathPart = 1,
            ServiceNamePart = 2
        }

        private string SettingsFilePath;
        public List<Settings> Settings = new List<Settings>();
        public bool IsActual
        {
            get;
            private set;
        }

        public GetSettings(string currentPath)
        {
            SettingsFilePath = Path.Combine(currentPath, "Settings.txt");
        }

        public void ReadSettingsFile()
        {
            IsActual = true;
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    using (StreamReader sr = new StreamReader(SettingsFilePath))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            var linePart = line.Split('|');
                            switch (linePart.Length)
                            {
                                case 1:
                                    Int32.TryParse(line, out _IntervalAllCile);
                                    MessageShowMethod.ShowMethod("Принята настройка временного интервала: " + _IntervalAllCile + " секунд.");
                                    break;
                                case 3:
                                    if (Directory.Exists(linePart[0].Trim()))
                                    {
                                        Settings.Add(new Settings(
                                            linePart[(int)SettingsLineParts.SourcePathPart].Trim(),
                                            linePart[(int)SettingsLineParts.BufferPathPart].Trim(),
                                            linePart[(int)SettingsLineParts.ServiceNamePart].Trim()));
                                        MessageShowMethod.ShowMethod("Принята настройка для " + linePart[(int)SettingsLineParts.ServiceNamePart].Trim());
                                    }
                                    else
                                    {
                                        MessageShowMethod.ShowMethod("Не удалось принять настройку для " + linePart[(int)SettingsLineParts.ServiceNamePart].Trim());
                                    }
                                    break;
                                default:
                                    MessageShowMethod.ShowMethod("Не верно задана настройка ");
                                    break;
                            }
                        }
                    }
                    if (Settings.Count > 0)
                    {
                        MessageShowMethod.ShowMethod("Настройки приняты успешно");
                    }
                    else
                    {
                        IsActual = false;
                    }
                }
                else
                {
                    MessageShowMethod.ShowMethod("Файл c настройками отсутствует");
                    IsActual = false;
                }
            }
            catch (Exception ex)
            {
                MessageShowMethod.ShowMethod(ex.Message);
                MessageShowMethod.ShowMethod("Ошибка при работе метода ReadPathsFromFile");
                IsActual = false;
            }
        }
    }
}
