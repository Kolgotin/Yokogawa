using System.IO;
using System.Text;

namespace LTDFilesDecryptor.Common
{
    public static class TxtLogger
    {
        private static StreamWriter Writer = new StreamWriter("log.txt", true, Encoding.Default);
        private static object Locker = new object();

        public static void Log(string str)
        {
            lock (Locker)
            {
                Writer.WriteLine(str);
            }
        }
    }
}
