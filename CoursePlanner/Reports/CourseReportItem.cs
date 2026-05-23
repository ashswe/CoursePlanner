using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursePlanner.Reports
{
    public class CourseReportItem : ReportItem
    {
        public string TermTitle { get; set; } = string.Empty;
        public int TermId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseStatus { get; set; } = string.Empty;
        public DateTime CourseDueDate { get; set; }

        public override string GetReportType()
        {
            return "Course List";
        }

        public override DateTime GetReportDate()
        {
            return DateTime.Now;
        }
    }
}
