using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Debugging
{
    public class ThreadParameters
    {
        public string message { get; private set; }
        public AutoResetEvent goingAheadThread { get; private set; }
        public AutoResetEvent currentThread { get; private set; }

        public ThreadParameters(string message, AutoResetEvent goingAheadThread, AutoResetEvent currentThread)
        {
            this.message = message;
            this.goingAheadThread = goingAheadThread;
            this.currentThread = currentThread;
        }
    }
}
