using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursePlanner.Reports
{
    public abstract class ReportItem
    {
        public string Title { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;

        public abstract string GetReportType();
        public abstract DateTime GetReportDate();
    }
}
