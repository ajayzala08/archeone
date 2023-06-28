﻿using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Req
{
    public class LeaveAddEditReqModel
    {
        public List<LeaveTypeMst> leaveTypeList { get; set; }
        public LeaveDetails leaveDetails { get; set; }
        public List<KeyValueModel> StartTimeList { get; set; }
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

    }
}