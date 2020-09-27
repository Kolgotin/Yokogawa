using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LTDFilesDecryptionLibrary
{
    public class WriteValuesToCSV
    {
        private const int MAX_ROWS_IN_CSV = 900_000;
        private Action ProgressAction;
        string FolderPath;
        List<string> SoughtPositions;
        List<SelectedFilesModel> Paths;
        object Locker = new object();

        public WriteValuesToCSV(string folderPath, List<SelectedFilesModel> paths, List<string> positions)
        {
            FolderPath = folderPath;
            SoughtPositions = positions;
            Paths = paths;
        }

        public void SetProgressAction(Action act)
        {
            ProgressAction = act;
        }

        public void Run(CancellationTokenSource cancelToken = null)
        {
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //
            //v
        }
    }
}
