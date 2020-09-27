using System;
using System.IO;
using System.Xml.Serialization;

namespace FileCollectorLibrary
{
    /// <summary>
    /// Служит для сохранения даты в xml файл
    /// </summary>
    public class DateToXml
    {
        string FilePath;

        /// <summary>
        /// Конструктор принимает путь к файлу, инициализирует объект для работы с файлом
        /// </summary>
        /// <param name="filePath">содержит путь к файлу</param>
        public DateToXml(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Если файл существует, десериализует хранящуюся в файле дату. Иначе возвращает нулевую дату
        /// </summary>
        /// <returns>Возвращает дату</returns>
        public DateTime GetDataFromFile()
        {
            var serializer = new XmlSerializer(typeof(DateTime));
            DateTime startDate = new DateTime();
            try
            {
                FileInfo lastDateTimeFile = new FileInfo(FilePath);
                if (File.Exists(FilePath))
                {
                    FileStream fs = lastDateTimeFile.OpenRead();
                    startDate = (DateTime)serializer.Deserialize(fs);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageShowMethod.ShowMethod(ex.Message);
                MessageShowMethod.ShowMethod("Ошибка при работе метода GetDataFromFile");
            }
            return startDate;
        }

        /// <summary>
        /// Принимает дату, записывает её в файл
        /// </summary>
        /// <param name="startDateCopy"></param>
        public void SetDateToFile(DateTime startDateCopy)
        {
            try
            {
                if(startDateCopy != null)
                {
                    FileInfo lastDateTimeFile = new FileInfo(FilePath);
                    FileStream fs;
                    var serializer = new XmlSerializer(typeof(DateTime));
                    if (lastDateTimeFile.Exists)
                    {
                        fs = lastDateTimeFile.Open(FileMode.Truncate, FileAccess.Write);
                    }
                    else
                    {
                        fs = lastDateTimeFile.Create();
                    }
                    lastDateTimeFile.Attributes = FileAttributes.Hidden;
                    serializer.Serialize(fs, startDateCopy);
                    fs.Close();
                }
            }
            catch(Exception ex)
            {
                MessageShowMethod.ShowMethod(ex.Message);
                MessageShowMethod.ShowMethod("Ошибка при работе метода SetDateToFile");
            }
        }
    }
}
