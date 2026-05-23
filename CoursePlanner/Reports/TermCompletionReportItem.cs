using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursePlanner.Reports
{
    public class TermCompletionReportItem : ReportItem
    {
        public string TermName { get; set; } = string.Empty;
        public int TermId { get; set; }
        public string DateRange { get; set; } = string.Empty;
        public int CompletionPercentage { get; set; }

        public override string GetReportType()
        {
            return "Term Completion";
        }

        public override DateTime GetReportDate()
        {
            return DateTime.Now;
        }
    }
}
