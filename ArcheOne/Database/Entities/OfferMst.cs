using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class OfferMst
{
    public int Id { get; set; }

    public int RequirementId { get; set; }

    public int ResumeId { get; set; }

    public int ScheduleInterviewId { get; set; }

    public int OfferStatusId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
