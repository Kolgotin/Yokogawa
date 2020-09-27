using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LTDFilesDecryptionLibrary
{
    public class FindTickets
    {
        private const string WINDOWS1251 = "Windows-1251";
        private Action ProgressAction;
        private string DirectoryPath;
        object Locker = new object();
        
        public FindTickets(string path)
        {
            DirectoryPath = path;
        }

        public void SetProgressAction(Action act)
        {
            ProgressAction = act;
        }

        public List<string> Run(List<SelectedFilesModel> fileNamesList, CancellationTokenSource cancelToken = null)
        {
            if (cancelToken is null)
            {
                cancelToken = new CancellationTokenSource();
            }

            var allTickets = new List<string>();
            Parallel.ForEach(fileNamesList, new ParallelOptions { CancellationToken = cancelToken.Token },
                (file) =>
                {
                    var tik = FindAllTickets(file.FullPath);
                    lock (Locker)
                    {
                        allTickets.AddRange(tik);
                    }
                    ProgressAction?.Invoke();
                });
            var res = allTickets.Distinct().ToList();
            return res;
        }

        private List<string> FindAllTickets(string path)
        {
            var allPositions = new List<string>();
            var input = new FileStream(path, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(input);
            var bytes = reader.ReadBytes((int)new FileInfo(path).Length);
            reader.Close();
            input.Close();
            string s1, s2;

            for (var rowindex = 0; rowindex <= 15; rowindex++)
            {
                int index1 = 872 + (rowindex * 0xa9200);
                int index2 = 888 + (rowindex * 0xa9200);

                for (var i = 1; i <= 8; i++)
                {
                    s1 = Encoding.GetEncoding(WINDOWS1251).GetString(bytes, index1 + 48 * (i - 1), 16).Replace(Convert.ToChar(0).ToString(), String.Empty);
                    s2 = Encoding.GetEncoding(WINDOWS1251).GetString(bytes, index2 + 48 * (i - 1), 8).Replace(Convert.ToChar(0).ToString(), String.Empty);
                    if (!(string.IsNullOrEmpty(s1)))
                        allPositions.Add($"{s1}.{s2}");
                }
            }
            return allPositions;
        }
    }
}
