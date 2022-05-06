using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RedCorners.Components
{
    public class Benchmark : IDisposable
    {
        public event EventHandler<TimeSpan> Stopped;

        readonly Stopwatch watch = new Stopwatch();

        public Benchmark()
        {
            watch.Start();
        }

        public void Dispose()
        {
            Stop();
        }

        public TimeSpan Stop()
        {
            if (!watch.IsRunning) return watch.Elapsed;
            watch.Stop();
            Stopped?.Invoke(this, watch.Elapsed);
            return watch.Elapsed;
        }

        public string StopToString()
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
