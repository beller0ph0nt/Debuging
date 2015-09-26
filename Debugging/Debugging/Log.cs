using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Debugging
{
    /// <summary>
    /// реализует асинхронную запись log-файлов
    /// </summary>
    public static class Log
    {
        private static readonly string _logPath;                    // путь до log-файлов
        private static AutoResetEvent _goingAheadThread = null;     // впереди идущий поток
        private static AutoResetEvent _goingBehindThread = null;    // позади идущий поток

        public static string LogPath { get { return _logPath; } }

        /// <summary>
        /// статический конструктор
        /// </summary>
        static Log()
        {
            try
            {
                _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// реализует асинхронную запись
        /// </summary>
        /// <param name="obj">передаваемыае потоку параметры</param>
        private static void WriteAsync(object obj)
        {
            try
            {
                var par = obj as ThreadParameters;
                DateTime dateTime = DateTime.Now;
                string filename = Path.Combine(_logPath, string.Format("{0}_{1:dd.MM.yyy}.log", AppDomain.CurrentDomain.FriendlyName, dateTime));
                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1}\r\n", dateTime, par.message);

                if (!Directory.Exists(_logPath))
                    Directory.CreateDirectory(_logPath);

                if (par.goingAheadThread != null)    // если есть впередиидущий поток
                    par.goingAheadThread.WaitOne();  // ожидаем когда поток освободится 

                File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));

                if (par.goingBehindThread != null)   // если есть позади идущий поток
                    par.goingBehindThread.Set();     // сообщаем потоку, что он может продолжить выполнение
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// записывает информацию в лог
        /// </summary>
        /// <param name="message">сообщение для записи</param>
        public static void Write(string message)
        {   // !!! по идее достаточно только переменной _goingBehindThread !!!
            _goingAheadThread = _goingBehindThread;             // прошлый поток устанавливаем впередиидущим
            _goingBehindThread = new AutoResetEvent(false);     // для текущего потока создаем новое событие автоматического сброса

            try
            {
                ThreadPool.QueueUserWorkItem(WriteAsync,
                                             new ThreadParameters(new string(message.ToCharArray()),
                                                                  _goingAheadThread,
                                                                  _goingBehindThread));
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// записывает информацию об исключении
        /// </summary>
        /// <param name="ex">исключение</param>
        public static void Write(Exception ex)
        {
            Write("Exception" +
                "    Data: " + ex.Data.ToString() + "\n" +
                "    HelpLink: " + ex.HelpLink + "\n" +
                "    InnerException: " + ex.InnerException + "\n" +
                "    Message: " + ex.Message + "\n" +
                "    Source: " + ex.Source + "\n" +
                "    StackTrace: " + ex.StackTrace + "\n" +
                "    StackTrace: " + ex.TargetSite);
        }

        /// <summary>
        /// ожидает завершения записи последнего потока
        /// </summary>
        public static void WaitWriteFinish()
        {
            if (_goingBehindThread != null)
                _goingBehindThread.WaitOne();
        }
    }
}
