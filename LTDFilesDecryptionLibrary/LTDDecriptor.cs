using System;
using System.Text;

namespace LTDFilesDecryptionLibrary
{
    class LTDDecriptor
    {
        //
        //
        //
        //
        int Index1, Index2, Rowindex, ValIndex;
        byte[] Bytes;

        private const string WINDOWS1251 = "Windows-1251";
        public LTDDecriptor(byte[] bytes, int rowindex)
        {
            //
            //
            Rowindex = rowindex;
            Bytes = bytes;
            //
        }

        public string GetTicketName(int columnNumber)
        {
            //
            //
            return $"//";
        }

        public DateTime GetDateTime(int increment)
        {
            //
            //
            return DateTime.Now;//
        }
        private DateTime ConvertTimestamp(int timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp);
        }

        public double GetValue(int columnNumber, int increment)
        {
            //
            return 0;//
        }
    }
}
