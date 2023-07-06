using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class LeaveMst
{
    public int Id { get; set; }

    public int LeaveTypeId { get; set; }

    public int AppliedByUserId { get; set; }

    public int ApprovedByUserId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public decimal? OpeningLeaveBalance { get; set; }

    public decimal? NoOfDays { get; set; }

    public decimal? PaidDays { get; set; }

    public decimal? UnPaidDays { get; set; }

    public string Reason { get; set; } = null!;

    public int LeaveStatusId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public bool? Hrstatus { get; set; }

    public bool? ProjectManagerStatus { get; set; }
}
