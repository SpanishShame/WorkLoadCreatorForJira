using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraWorkloadReportCreator.Entity
{
    public class Report
    {
        public string Author { get; set; }
        public double Sum { get; set; }
        public WorkTask[] Tasks { get; set; }
    }

    public class WorkTask
    {
        public double WorkTime { get; set; }
        public string TaskId { get; set; }
        public DateTime Date {get; set; }
        public string Comment { get; set; }
    }
}
