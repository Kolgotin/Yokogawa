using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LTDFilesDecryptionLibrary
{
    public class CalculateAverageValues
    {
        private const int MAX_ROWS_IN_CSV = 900_000;
        private Action ProgressAction;
        private List<TicketInfo> Tickets;
        private Dictionary<StringDateTimeKey, TicketAtRange> AverageValsDictionary;
        string FolderPath;
        object StrangeLocker = new object();
        object AverageLocker = new object();
        RangePeriod AccuracyPeriod;
        string[] DecriptedFiles;

        public CalculateAverageValues(string folderPath, List<TicketInfo> tickets, RangePeriod averagePeriod = 0)
        {
            FolderPath = folderPath;
            Tickets = tickets;
            AccuracyPeriod = averagePeriod;
            AverageValsDictionary = new Dictionary<StringDateTimeKey, TicketAtRange>();
        }

        public int FindFiles()
        {
            DecriptedFiles = Directory.GetFiles($@"{FolderPath}\values", "*.csv", SearchOption.TopDirectoryOnly);
            return DecriptedFiles.Count();
        }

        public void SetProgressAction(Action act)
        {
            ProgressAction = act;
        }

        public void Run( CancellationTokenSource cancelToken = null)
        {
            if (cancelToken is null)
            {
                cancelToken = new CancellationTokenSource();
            }

            if (!Directory.Exists(($@"{FolderPath}\AverageVals\")))
            {
                Directory.CreateDirectory(($@"{FolderPath}\AverageVals\"));
            }

            Dictionary<int, int> threadsRowsCount = new Dictionary<int, int>();
            Parallel.ForEach(DecriptedFiles, new ParallelOptions { CancellationToken = cancelToken.Token }, (filePath) =>
            //paths.ToList().ForEach(filePath => 
            {
                int strangeValuesFileNumber = 0;
                int threadId = Thread.CurrentThread.ManagedThreadId;
                lock (StrangeLocker)
                {
                    if (!threadsRowsCount.ContainsKey(threadId))
                    {
                        threadsRowsCount.Add(threadId, 0);
                    }
                    strangeValuesFileNumber = threadId * 10_000 + threadsRowsCount[threadId] / MAX_ROWS_IN_CSV;
                }

                StreamWriter strangeValuesWriter = new StreamWriter($@"{FolderPath}\AverageVals\strangeValues{strangeValuesFileNumber}.csv", true, Encoding.Default);

                StreamReader sr = new StreamReader(filePath, Encoding.Default);
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                string buffer = sr.ReadLine();
                while (!(buffer is null))
                {
                    var strParts = buffer.Split(';');
                    string dateStr = strParts[0];
                    DateTime dateVal;
                    bool b = DateTime.TryParse(dateStr, out dateVal);
                    if (b && dateVal > new DateTime(1980, 1, 1))
                    {
                        double dValue = Convert.ToDouble(strParts[1]);
                        TicketInfo ticketName = Tickets.FirstOrDefault(x=> x.TicketName == strParts[2]);
                        if(!(ticketName is null))
                        {
                            StringDateTimeKey key = new StringDateTimeKey(ticketName.TicketName, dateVal, AccuracyPeriod);
                            TicketAtRange pair;
                            lock (AverageLocker)
                            {
                                if (AverageValsDictionary.ContainsKey(key))
                                {
                                    pair = AverageValsDictionary[key];
                                }
                                else
                                {
                                    pair = new TicketAtRange(ticketName, dateVal);
                                    AverageValsDictionary.Add(key, pair);
                                }
                            }

                            bool doneMerge = pair.Merge(dValue);
                            if (!doneMerge)
                            {
                                string err = $"{fileName}; {dateStr}; {key.Name}; {dValue}";
                                strangeValuesWriter.WriteLine(err);
                                lock (StrangeLocker)
                                {
                                    threadsRowsCount[threadId]++;
                                }
                            }
                        }
                    }
                    buffer = sr.ReadLine();
                }

                strangeValuesWriter.Close();
                sr.Close();
                ProgressAction?.Invoke();
            });

            SaveResulsToDifFiles();
        }

        private void SaveResulsToDifFiles()
        {
            var avPairs = AverageValsDictionary.GroupBy(x => x.Key.Name);

            avPairs.ToList().ForEach(ticket =>
            {
                string ticketFileName = $@"{FolderPath}\AverageVals\AverageVals {ticket.Key}.csv";
                StreamWriter averageValuesWriter = new StreamWriter(ticketFileName, false, Encoding.Default);
                averageValuesWriter.WriteLine("DateTime; Value; Count; StrangeCount; Average");

                var averageValuesList = ticket.OrderBy(x=>x.Key.DateTimeValue).ToList();
                averageValuesList.ForEach(x =>
                {
                    double res = x.Value.CorrectCount > 0 ? x.Value.SumValue / x.Value.CorrectCount : 0;
                    averageValuesWriter.WriteLine($"{x.Key.DateTimeValue};{x.Value.SumValue};{x.Value.CorrectCount};{x.Value.StrangeCount}; {res}");
                });
                averageValuesWriter.Close();
            });
        }

        private void SaveResultsTofile()
        {
            string newFileName = $@"{FolderPath}\AverageVals\AverageVals.csv";
            StreamWriter averageValuesWriter = new StreamWriter(newFileName, false, Encoding.Default);
            averageValuesWriter.WriteLine("Ticket; Value; Count; StrangeCount");

            var averageValuesList = AverageValsDictionary.Values.ToList();

            averageValuesList.ForEach(x =>
            {
                averageValuesWriter.WriteLine($"{x.TicketName};{x.SumValue};{x.CorrectCount};{x.StrangeCount}");
            });

            averageValuesWriter.WriteLine();
            averageValuesWriter.WriteLine("Ticket; Average");

            averageValuesList.ForEach(x =>
            {
                double res = x.CorrectCount > 0 ? x.SumValue / x.CorrectCount : 0;
                averageValuesWriter.WriteLine($"{x.TicketName}; {res}");

            });
            averageValuesWriter.Close();
        }
    }
}
