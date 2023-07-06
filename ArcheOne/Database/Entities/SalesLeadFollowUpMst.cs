using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class SalesLeadFollowUpMst
{
    public int Id { get; set; }

    public int SalesLeadId { get; set; }

    public int SalesContactPersonId { get; set; }

    public DateTime FollowUpDateTime { get; set; }

    public DateTime? NextFollowUpDateTime { get; set; }

    public int SalesLeadStatusId { get; set; }

    public int SalesLeadActionId { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public int? NextFollowUpActionId { get; set; }

    public string? NextFollowUpNotes { get; set; }
}
