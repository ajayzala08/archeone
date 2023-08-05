namespace ArcheOne.Models.Res
{
    public class DashboardDetailsResModel
    {
        public int InterviewRoundCount { get; set; }
        public int UserCount { get; set; }
        public int SalesLeadsCount { get; set; }
        public int ProjectCount { get; set; }
        public int ProjectCompletedCount { get; set; }
        public int ProjectToDoCount { get; set; }
        public int ProjectInProgressCount { get; set; }
        public int SalesLeadOpportunityCount { get; set; }
        public int ClosureCount { get; set; }
        public int SubmissionCount { get; set; }
        public int BDCount { get; set; }
        public int TeamCount { get; set; }
        public int UncheckedLeave { get; set; }
        public int PendingResumeApprovalCount { get; set; }
        public int InHouseRequirementCount { get; set; }
        public int ClientRequirementCount { get; set; }
        public int ActiveRequirementCount { get; set; }
        public int InActiveRequirementCount { get; set; }
        public int OnHoldRequirementCount { get; set; }
        public int CloseRequirementCount { get; set; }
        public bool IsUserHR { get; set; }
        public bool IsUserSD { get; set; }
        public bool IsUserQA { get; set; }
        public bool IsUserDesigner { get; set; }
        public bool IsUserSales { get; set; }
        public bool IsUserRecruitment { get; set; }

        public int SalesLeadNewCount { get; set; }
        public int SalesLeadDNCCount { get; set; }
        public int SalesLeadInProgressCount { get; set; }
        public int SalesLeadNotInterestedCount { get; set; }
        public int NextFollowUpCount { get; set; }
        public int TotalRequirementCount { get; set; }
        public int OfferCount { get; set; }
        public int JoiningCount { get; set; }
        public int TaskReportCount { get; set; }
        public int AppraisalRatingCompletedCount { get; set; }
        public int AppraisalRatingInprogressCount { get; set; }
        public int RecentJoiningCount { get; set; }
        public List<TaskDetails> PerviousDayTaskCount { get; set; }
        public List<DataPoint> DataPoint { get; set; }
    }

    public class DataPoint
    {
        public string Label { get; set; }
        public string LegendText { get; set; }
        public double Y { get; set; }
    }
    public class TaskDetails
    {
        public string UserName { get; set; }
        public string Task { get; set; }
        public string Date { get; set; }
    }
}
