using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class InterviewRoundStatusMst
{
    public int Id { get; set; }

    public string InterviewRoundStatusName { get; set; } = null!;

    public string InterviewRoundStatusCode { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
