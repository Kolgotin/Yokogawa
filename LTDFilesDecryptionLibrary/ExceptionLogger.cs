using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTDFilesDecryptionLibrary
{
    public class ExceptionLogger
    {
        Action<string> LoggerAction;
        public ExceptionLogger(Action<string> action)
        {
            LoggerAction = action;
        }

        public void LogException(Exception ex)
        {
            var str = ConvertExceptionToString(ex);
            LoggerAction(str);
        }

        private string ConvertExceptionToString(Exception ex, int deep = 0)
        {
            StringBuilder sb = new StringBuilder();
            string prefix = new string(' ', deep * 4);
            sb.Append($"{prefix}DateTime: {DateTime.Now}\r\n");
            sb.Append($"{prefix}Message: {ex.Message}\r\n");
            sb.Append($"{prefix}StackTrace: {ex.StackTrace}\r\n");

            var inEx = ex.InnerException;
            if (!(inEx is null))
            {
                sb.Append(ConvertExceptionToString(inEx, deep + 1));
            }

            return sb.ToString();
        }
    }
}
