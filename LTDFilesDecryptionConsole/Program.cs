using System;
using LTDFilesDecryptionLibrary;

namespace LTDFilesDecryptionConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Throw();
            }
            catch (Exception ex)
            {
                ExceptionLogger exceptionLogger = new ExceptionLogger(Console.WriteLine);
                exceptionLogger.LogException(ex);
            }

            Console.ReadKey();
        }

        private static void Throw()
        {
            try
            {
                Throw2();
            }
            catch (Exception ex2)
            {
                var ex = new Exception("aaa", ex2);
                throw ex;
            }
        }
        private static void Throw2()
        {
            var ex = new Exception("bbbbbbbbbbb");
            throw ex;
        }
    }
}
