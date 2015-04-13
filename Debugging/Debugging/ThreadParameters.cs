using System.Threading;

namespace Debugging
{
    /// <summary>
    /// Класс параметров, передаваемых потоку
    /// </summary>
    public class ThreadParameters
    {
        #region Свойства

        /// <summary>
        /// Текстовое сообщение, передаваемое потоку
        /// </summary>
        public string message { get; private set; }

        /// <summary>
        /// Событие автоматического сброса для впереди идущего потока
        /// </summary>
        public AutoResetEvent goingAheadThread { get; private set; }

        /// <summary>
        /// Событие автоматического сброса для позади идущего потока
        /// </summary>
        public AutoResetEvent goingBehindThread { get; private set; }

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="message">Сообщение, передаваемое потоку</param>
        /// <param name="goingAheadThread">Событие автоматического сброса для впереди идущего потока</param>
        /// <param name="goingBehindThread">Событие автоматического сброса для позади идущего потока</param>
        public ThreadParameters(string message, AutoResetEvent goingAheadThread, AutoResetEvent goingBehindThread)
        {
            this.message = message;
            this.goingAheadThread = goingAheadThread;
            this.goingBehindThread = goingBehindThread;
        }
    }
}
