using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class UnapprovedBatchReportModel
    {
        public List<RecievingAgentItem> Agents { get; internal set; }
    }
}