using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RedCorners.Components
{
    public class Benchmark
    {
        readonly Stopwatch watch = new Stopwatch();

        public Benchmark()
        {
            watch.Start();
        }

        public TimeSpan Stop()
        {
            watch.Stop();
            return watch.Elapsed;
        }

        public string StopToString()
        {
            watch.Stop();
            return ToString();
        }

        public override string ToString()
        {
            return watch.Elapsed.TotalSeconds.ToString("N2") + "s";
        }
    }
}
