using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Debugging
{
    public class Log
    {
        //private static object _locker = new object();
        private static AutoResetEvent _goingAheadThread;    // Впередиидущий поток
        private static AutoResetEvent _currentThread;       // Текущий поток

        private static void WriteAsync(object obj)
        {
            try
            {
                var currentDateTime = new DateTime(DateTime.Now.Ticks);
                var tmpPar = obj as ThreadParameters;

                if (tmpPar.goingAheadThread != null)
                    tmpPar.goingAheadThread.WaitOne();

                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");

                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                string filename = Path.Combine(logPath, string.Format("{0}_{1:dd.MM.yyy}.log", AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1}\r\n", currentDateTime, tmpPar.message);

                //lock (_locker)
                //{
                File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                //}

                if (tmpPar.currentThread != null)
                    tmpPar.currentThread.Set();
            }
            catch (Exception ex) { throw ex; }
        }

        public static void Write(string message)
        {
            _goingAheadThread = _currentThread;
            _currentThread = new AutoResetEvent(false);
            ThreadPool.QueueUserWorkItem(WriteAsync, 
                new ThreadParameters(new string(message.ToCharArray()), _goingAheadThread, _currentThread));
        }
    }
}
