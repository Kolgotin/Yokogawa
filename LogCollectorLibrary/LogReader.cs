using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace LogCollectorLibrary
{
    /// <summary>
    /// находит все файлы формата .log в папке с путём SourcePath, включая вложенные папки
    /// считывает их и записывает данные из них в базу данных
    /// </summary>
    public class LogReader
    {
        enum AlarmLogFileYokogawaColumns
        {

            DateTimeUMTEvent = 0,
            UnknownColumn1 = 1,
            TypeEvent = 2,
            IindexNumberEvent = 3,
            Writer = 4,
            ExactDateTime = 5,
            UnknownColumn2 = 6,
            UnknownColumn3 = 7,
            ControllersAndStations = 8,
            PositionOrAlarmOrBlockName = 9,
            MessageText = 11
        }

        private string ConnectionString = @"data source=TM-SQL.zos-v.ru;initial catalog=Yokogawa;integrated security=True;multipleactiveresultsets=True;";
        private string SourcePath;
        
        private DBMethods DBConnetcor;

        private List<ProductType> ProductTypeList = new List<ProductType>();
        private List<ControllersAndStations> ControllersAndStationsList = new List<ControllersAndStations>();
        private List<UnknownColumn1> UnknownColumn1List = new List<UnknownColumn1>();
        private List<Writers> WritersList = new List<Writers>();


        public LogReader(string sourcePath)
        {
            SourcePath = sourcePath;
            DBConnetcor = new DBMethods(ConnectionString);
            ReadDBTables();
            GetAllFolders();
        }

        public void ReadFiles()
        {            
            foreach(var prod in ProductTypeList)
            {
                List<LogFileNameAndPath> logsInFolder = GetFilesFromFolder(prod.Path);
                var dates = logsInFolder.Select(x => x.LogsDate).Distinct().ToList();
                dates.ForEach(x => ReadCurrentDate(x, prod.ProductTypeId, logsInFolder));
            }
        }

        private void ReadCurrentDate(string currentDate, int productTypeId, List<LogFileNameAndPath> logsInFolder)
        {
            List<YokogawaLog> logReaderCurerntDay = new List<YokogawaLog>();
            logsInFolder.Where(y => y.LogsDate == currentDate).ToList().ForEach(z =>
                ReadCurrentFile(z.LogsFullPath, logReaderCurerntDay)
                );

            DBConnetcor.BulkCopyInsert(productTypeId, logReaderCurerntDay);
            MessageShowMethod.ShowMethod(DateTime.Now + " Id: "+ productTypeId + " Date: " + currentDate);
        }

        private void ReadDBTables()
        {
            ProductTypeList = DBConnetcor.ReadProductTypeTable();
            ControllersAndStationsList = DBConnetcor.ReadControllersAndStationsTable();
            UnknownColumn1List = DBConnetcor.ReadUnknownColumn1Table();
            WritersList = DBConnetcor.ReadWriterTable();
        }

        private void GetAllFolders()
        {
            try
            {
                var directories = Directory.GetDirectories(SourcePath);
                foreach (var dir in directories)
                {
                    ParseProductType(dir);
                }
            }
            catch (Exception ex)
            {
                MessageShowMethod.ShowMethod("Ошибка при выполнении метода GetAllFolders");
            }
        }

        private List<LogFileNameAndPath> GetFilesFromFolder(string sourcePath)
        {
            List<LogFileNameAndPath> logsInFolder = new List<LogFileNameAndPath>();
            try
            {
                Directory.GetFiles(sourcePath, "*.log", SearchOption.AllDirectories).ToList()
                    .ForEach(filePath =>
                    {
                        string fileName = Path.GetFileName(filePath);
                        string fileDate = fileName.Split('-', '.')[1];

                        logsInFolder.Add(new LogFileNameAndPath()
                        {
                            LogsFullPath = filePath,
                            LogsFileName = fileName,
                            LogsDate = fileDate
                        });
                    });
                return logsInFolder;
            }
            catch (Exception ex)
            {
                MessageShowMethod.ShowMethod(ex.Message);
                return null;
            }
        }

        private void ReadCurrentFile(string currentFilePath, List<YokogawaLog> logReaderCurerntDay)
        {
            try
            {
                var stringsFromFile = File.ReadLines(currentFilePath, Encoding.Default).ToList();
                stringsFromFile.ForEach(x => ParseAndWriteToList(x, logReaderCurerntDay));
            }
            catch (Exception ex)
            {
                MessageShowMethod.ShowMethod("Ошибка при выполнении метода ReadCurrentFile");
            }
        }

        private void ParseAndWriteToList(string currentLine, List<YokogawaLog> logReaderCurerntDay)
        {
            var lineParts = currentLine.Split(',');
            for (int i = (int)AlarmLogFileYokogawaColumns.MessageText + 1; i < lineParts.Count(); i++)
            {
                lineParts[(int)AlarmLogFileYokogawaColumns.MessageText] += "," + lineParts[i];
            }

            var currentParsedLine = new YokogawaLog()
            {
                DateTimeUMTEvent = DateTime.ParseExact(lineParts[(int)AlarmLogFileYokogawaColumns.DateTimeUMTEvent],
                    @"yyyy/MM/dd HH:mm:ss zzz", System.Globalization.CultureInfo.InvariantCulture),
                //UnknownColumn1 = lineParts[(int)AlarmLogFileYokogawaColumns.UnknownColumn1],
                UnknownColumn1Id = ParseUnknownColumn1(lineParts[(int)AlarmLogFileYokogawaColumns.UnknownColumn1]),
                TypeEvent = Convert.ToInt32(lineParts[(int)AlarmLogFileYokogawaColumns.TypeEvent]),
                IindexNumberEvent = Convert.ToInt32(lineParts[(int)AlarmLogFileYokogawaColumns.IindexNumberEvent]),
                //WhoDoesProgOrOperator = lineParts[(int)AlarmLogFileYokogawaColumns.Writer],
                WriterId = ParseWriter(lineParts[(int)AlarmLogFileYokogawaColumns.Writer]),
                ExactDateTime = DateTime.ParseExact(lineParts[(int)AlarmLogFileYokogawaColumns.ExactDateTime], @"yyyy/MM/dd HH:mm:ss.fff zzz",
                    System.Globalization.CultureInfo.InvariantCulture),
                UnknownColumn2 = lineParts[(int)AlarmLogFileYokogawaColumns.UnknownColumn2],
                UnknownColumn3 = lineParts[(int)AlarmLogFileYokogawaColumns.UnknownColumn3],
                //ControllersAndStations = lineParts[(int)AlarmLogFileYokogawaColumns.ControllersAndStations],
                ControllersAndStationsId = ParseControllersAndStations(lineParts[(int)AlarmLogFileYokogawaColumns.ControllersAndStations]),
                PositionOrAlarmOrBlockName = lineParts[(int)AlarmLogFileYokogawaColumns.PositionOrAlarmOrBlockName],
                MessageText = lineParts[(int)AlarmLogFileYokogawaColumns.MessageText]
            };

            logReaderCurerntDay.Add(currentParsedLine);
        }

        private int ParseUnknownColumn1(string value)
        {
            if (value == "")
                return 0;

            int unknownColumn1Id;
            var currentUC1 = UnknownColumn1List.FirstOrDefault(x => x.UnknownColumn1Type == value);
            if (currentUC1 == null)
            {
                unknownColumn1Id = DBConnetcor.InsertUnknownColumn1(value);
                UnknownColumn1List.Add(new UnknownColumn1()
                {
                    UnknownColumn1Id = unknownColumn1Id,
                    UnknownColumn1Type = value
                });
            }
            else
            {
                unknownColumn1Id = currentUC1.UnknownColumn1Id;
            }
            return unknownColumn1Id;
        }

        private int ParseWriter(string value)
        {
            if (value == "")
                return 0;

            int writerId;
            var currentWriter = WritersList.FirstOrDefault(x => x.WriterName == value);
            if (currentWriter == null)
            {
                writerId = DBConnetcor.InsertWriter(value);
                WritersList.Add(new Writers
                {
                    WriterId = writerId,
                    WriterName = value
                });
            }
            else
            {
                writerId = currentWriter.WriterId;
            }
            return writerId;
        }

        private int ParseControllersAndStations(string value)
        {
            if (value == "")
                return 0;

            int controllersAndStationsId;
            var currentCAS = ControllersAndStationsList.FirstOrDefault(x => x.ControllersAndStationsName == value);                
            if (currentCAS == null)
            {
                controllersAndStationsId = DBConnetcor.InsertControllersAndStations(value);

                ControllersAndStationsList.Add(new ControllersAndStations()
                {
                    ControllersAndStationsId = controllersAndStationsId,
                    ControllersAndStationsName = value
                });
            }
            else
            {
                controllersAndStationsId = currentCAS.ControllersAndStationsId;
            }
            return controllersAndStationsId;
        }

        private int ParseProductType(string value)
        {
            if (value == "")
                return 0;

            int productTypeId;
            string dirName = value.Replace(SourcePath, "").Replace(@"\", "");
            var currentProd = ProductTypeList.FirstOrDefault(x => x.ProductName == dirName);
            if (currentProd == null)
            {
                productTypeId = DBConnetcor.InsertProductType(dirName);
                ProductTypeList.Add(new ProductType()
                {
                    ProductTypeId = productTypeId,
                    ProductName = dirName,
                    Path = value
                });
            }
            else
            {
                productTypeId = currentProd.ProductTypeId;
                currentProd.Path = value;
            }
            return productTypeId;
        }
    }
}
