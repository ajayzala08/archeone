namespace ArcheOne.Models.Res
{
    public class LeavesListResModel
    {
        public List<LeaveDetailsList> LeaveDetailsLists { get; set; }


    }
    public class LeaveDetailsList
    {
        public int Id { get; set; }

        public string LeaveTypeName { get; set; }
        public string AppliedByUserName { get; set; }
        public string ApprovedByHRUserId { get; set; }
        public string ApprovedByReportingUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal NoOfDays { get; set; }
        public decimal PaidDays { get; set; }
        public decimal UnPaidDays { get; set; }
        public string Reason { get; set; }
        public string LeaveStatus { get; set; }
        public string BalanceMonth { get; set; }
        public decimal BalanceYear { get; set; }
        public decimal SickLeaveBalance { get; set; }
        public decimal SickLeaveTaken { get; set; }
        public decimal CasualLeaveBalance { get; set; }
        public decimal CasualLeaveTaken { get; set; }
        public decimal EarnedLeaveBalance { get; set; }
        public decimal EarnedLeaveTaken { get; set; }
        public string Details { get; set; }
        public string HrStatus { get; set; }
        public string ApprovedByReportingStatus { get; set; }
        public decimal LeaveTaken { get; set; }
        public bool EditDisable { get; set; }
        public bool HREditDisable { get; set; }
    }
}
