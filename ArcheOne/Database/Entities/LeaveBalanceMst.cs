using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class LeaveBalanceMst
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int LeaveTypeId { get; set; }

    public decimal? OpeningLeaveBalance { get; set; }

    public decimal? NoOfDays { get; set; }

    public decimal? ClosingLeaveBalance { get; set; }

    public string BalanceMonth { get; set; } = null!;

    public decimal? BalanceYear { get; set; }

    public DateTime BalanceDate { get; set; }

    public decimal? SickLeaveBalance { get; set; }

    public decimal? SickLeaveTaken { get; set; }

    public decimal? CasualLeaveTaken { get; set; }

    public decimal? CasualLeaveBalance { get; set; }

    public decimal? EarnedLeaveTaken { get; set; }

    public decimal? EarnedLeaveBalance { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public decimal? PaidDayLeaves { get; set; }

    public decimal? UnPaidDayLeaves { get; set; }
}
