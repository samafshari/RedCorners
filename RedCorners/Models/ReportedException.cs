using System;
using System.Collections.Generic;
using System.Text;

namespace RedCorners.Models
{
    public class ReportedException
    {
        public Exception Exception { get; set; }
        public string Caller { get; set; }
    }
}
