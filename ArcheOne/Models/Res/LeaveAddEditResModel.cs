﻿using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Res
{
    public class LeaveAddEditResModel
    {
        public List<LeaveTypeMst> LeaveTypeList { get; set; }
        public LeaveDetails LeaveDetails { get; set; }
        public List<KeyValueModel> StartTimeList { get; set; }
        public List<KeyValueModel> LeaveStatusList { get; set; }
        public bool LeaveStatusChangeView { get; set; }
    }

    public class LeaveDetails
    {
        public int Id { get; set; }

        public int LeaveTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string Reason { get; set; } = null!;

        public int LeaveStatusId { get; set; }

        public int HrStatus { get; set; }

        public int ApprovedByReportingStatus { get; set; }

        public DateTime JoiningDate { get; set; }

    }
}