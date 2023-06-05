using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class ScheduleInterviewMst
{
    public int Id { get; set; }

    public int ResumeFileUploadId { get; set; }

    public int RequirementId { get; set; }

    public int InterviewStatusId { get; set; }

    public int InterviewTypeStatusId { get; set; }

    public DateTime InterviewDate { get; set; }

    public string CandidateName { get; set; } = null!;

    public string InterviewBy { get; set; } = null!;

    public string InterviewLocation { get; set; } = null!;

    public string Note { get; set; } = null!;

    public int StatusId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
