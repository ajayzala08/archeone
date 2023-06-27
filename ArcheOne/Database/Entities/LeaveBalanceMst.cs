using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class LeaveBalanceMst
{
    public int Id { get; set; }

    public string LeaveStatus { get; set; } = null!;

    public int UserId { get; set; }

    public int LeaveTypeId { get; set; }

    public decimal? PendingLeaveBalance { get; set; }

    public decimal? AvailableLeaveBalance { get; set; }

    public decimal? TotalLeaveBalance { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
