using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Debugging
{
    /// <summary>
    /// Класс, реализующий асинхронную запись log-файлов
    /// </summary>
    public class Log
    {
        private static AutoResetEvent _goingAheadThread = null;     // Впереди идущий поток
        private static AutoResetEvent _goingBehindThread = null;    // Позади идущий поток

        /// <summary>
        /// Метод, реализующий асинхронную запись
        /// </summary>
        /// <param name="obj">Класс, передаваемых потоку параметров</param>
        private static void WriteAsync(object obj)
        {
            try
            {
                var tmpPar = obj as ThreadParameters;
                // !!! Возможно данную переменную необходимо вынести как константу !!!
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");

                if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

                string filename = Path.Combine(logPath, string.Format("{0}_{1:dd.MM.yyy}.log", AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1}\r\n", DateTime.Now, tmpPar.message);

                // Если есть впередиидущий поток, то ожидаем когда он освободится 
                if (tmpPar.goingAheadThread != null) tmpPar.goingAheadThread.WaitOne();

                File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));

                // Если есть позади идущий поток, то сообщаем ему, что он может продолжить выполнение
                if (tmpPar.goingBehindThread != null) tmpPar.goingBehindThread.Set();
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Метод, вызывающий асинхронную запись
        /// </summary>
        /// <param name="message">Сообщение для log-файла</param>
        public static void Write(string message)
        {   // !!! По иде достаточно только переменной _goingBehindThread !!!
            _goingAheadThread = _goingBehindThread;             // Прошлый поток устанавливаем впередиидущим
            _goingBehindThread = new AutoResetEvent(false);     // Для текущего потока создаем новое событие автоматического сброса
            ThreadPool.QueueUserWorkItem(WriteAsync,
                                         new ThreadParameters(new string(message.ToCharArray()),
                                                              _goingAheadThread,
                                                              _goingBehindThread));
        }
    }
}
