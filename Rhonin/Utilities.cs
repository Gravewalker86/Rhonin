using System;
using System.Collections.Generic;
using System.Text;

namespace Rhonin.Utilities//Utility classes to make testing / common tasks easier.
{
    using System.Diagnostics;

    class Timer//Class made to reduce the amount of crap needed for simple timers. May remove in future.
    {
        private Stopwatch _timer;

        public Timer()
        {
            _timer = new Stopwatch();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Clear()
        {
            _timer.Reset();
        }

        public double TimeInMilliseconds()
        {
            return _timer.Elapsed.TotalMilliseconds;
        }

        public double TimeInNanoseconds()
        {
            return _timer.Elapsed.TotalMilliseconds * 1000000;
        }

        public string TimeInMilisecondsToString()
        {
            return ($"{_timer.Elapsed.Milliseconds} Milliseconds, "+
                $"{(_timer.Elapsed.TotalMilliseconds - _timer.Elapsed.Milliseconds) * 1000000} "+
                "Nanoseconds.");
        }
        
    }
}
