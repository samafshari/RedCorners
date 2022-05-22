using System;
using System.Collections.Generic;
using System.Text;

namespace RedCorners.Models
{
    public class Interval<T>
    {
        public Interval(T start, T end)
        {
            Start = start;
            End = end;
        }

        public T Start { get; set; }
        public T End { get; set; }

        public override string ToString()
        {
            return $"{Start} -> {End}";
        }
    }
}
