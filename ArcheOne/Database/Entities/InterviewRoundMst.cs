using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class InterviewRoundMst
{
    public int Id { get; set; }

    public int InterviewId { get; set; }

    public int InterviewRoundStatusId { get; set; }

    public int InterviewRoundTypeId { get; set; }

    public DateTime InterviewStartDateTime { get; set; }

    public DateTime InterviewEndDateTime { get; set; }

    public string InterviewBy { get; set; } = null!;

    public string InterviewLocation { get; set; } = null!;

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
