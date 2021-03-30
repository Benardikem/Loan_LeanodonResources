using System;
using System.Collections.Generic;

namespace Web.Models
{
    public class DashboardViewModel
    {
        public decimal ShareRate { get; internal set; }
        public int Shareholders { get; internal set; }
        public long Shares { get; internal set; }
        public decimal ShareAmount { get; internal set; }
        public List<RecentActivity> RecentActivities { get; internal set; }
        public string LastActivityDuration { get; internal set; }
        public int SuccessfulReturnsCount { get; internal set; }
        public int AwaitingApprovalReturnsCount { get; internal set; }
        public int RejectedReturnsCount { get; internal set; }
        public int PendingReturnsCount { get; internal set; }
        public long Unapproved { get; internal set; }
        public long Qty { get; internal set; }
        //public string ProgressWidth { get; internal set; }
        //public string ProgressWidthClass { get; internal set; }
        public int PendingSubmissionSummaryCount { get; internal set; }
        public Decimal TotalPendingShares { get; internal set; }
        public Decimal TotalPendingAmount { get; internal set; }
    }

    public class RecentActivity
    {
        public string Title { get; internal set; }
        public string Message { get; internal set; }
        public string Duration { get; internal set; }
        public string Class { get; internal set; }
    }
}