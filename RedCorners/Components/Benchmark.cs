using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RedCorners.Components
{
    public class Benchmark : IDisposable
    {
        public event EventHandler<TimeSpan> Stopped;
        
        public TimeSpan Elapsed => watch.Elapsed;

        protected readonly Stopwatch watch = new Stopwatch();

        public Benchmark()
        {
            watch.Start();
        }

        public virtual void Dispose()
        {
            Stop();
        }

        public virtual TimeSpan Stop()
        {
            if (!watch.IsRunning) return watch.Elapsed;
            watch.Stop();
            Stopped?.Invoke(this, watch.Elapsed);
            return watch.Elapsed;
        }

        public virtual string StopToString()
        {
            Stop();
            return ToString();
        }

        public override string ToString()
        {
            return watch.Elapsed.TotalSeconds.ToString("N2") + "s";
        }
    }
}
