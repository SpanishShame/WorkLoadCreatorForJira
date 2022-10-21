using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JiraWorkloadReportCreator.Entity
{
    public class JiraConfig
    {
        public string JiraUrl { get; set; }
        public string JiraTaskUrl { get; set; }
        public string JiraIssueUrl { get; set; }
        public string JiraLogin { get; set; }
        public string JiraToken { get; set; }
        public string ReportPath { get; set; }
        public DateTime ReportStartDate { get; set; }
    }
}
