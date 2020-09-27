using LTDFilesDecryptionLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTDFilesDecryptor.Common
{
    public static class ExtensionClass
    {
        public static string RangePeriodToString(this RangePeriod period)
        {
            switch (period)
            {
                case RangePeriod.Hour:
                    return "Час";
                case RangePeriod.Day:
                    return "День";
                case RangePeriod.Month:
                    return "Месяц";
                case RangePeriod.AllTime:
                    return "Всё время";
                default:
                    return "";
            }
        }
    }
}
