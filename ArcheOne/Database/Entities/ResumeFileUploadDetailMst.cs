using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class ResumeFileUploadDetailMst
{
    public int Id { get; set; }

    public int RecruitmentJobId { get; set; }

    public string ApplicantName { get; set; } = null!;

    public DateTime ApplicantDob { get; set; }

    public string Address { get; set; } = null!;

    public int? Count { get; set; }

    public int? AlterCount { get; set; }

    public string Email { get; set; } = null!;

    public int TotalExperience { get; set; }

    public int RelevantExperience { get; set; }

    public string CurrentCompany { get; set; } = null!;

    public string CurrentDesignation { get; set; } = null!;

    public int? NoticePeriod { get; set; }

    public bool? CanJoin { get; set; }

    public decimal Ctc { get; set; }

    public decimal Ectc { get; set; }

    public string Reason { get; set; } = null!;

    public bool? AnyInterviewOffer { get; set; }

    public string Education { get; set; } = null!;

    public string CurrentLocation { get; set; } = null!;

    public string PrefferedLocation { get; set; } = null!;

    public string Native { get; set; } = null!;

    public string ResumeName { get; set; } = null!;

    public int ResumeStatusId { get; set; }

    public string Skills { get; set; } = null!;

    public int FamilyCount { get; set; }

    public int FriendCount { get; set; }

    public string ReasonGap { get; set; } = null!;

    public decimal CurrentTakeHome { get; set; }

    public decimal? CurrentDrawing { get; set; }

    public DateTime LastSalaryHike { get; set; }

    public decimal ExpectedTakeHome { get; set; }

    public decimal? ExpectedDrawing { get; set; }

    public string HikeReason { get; set; } = null!;

    public string HowJoinEarlyReason { get; set; } = null!;

    public string ReasonForJoin { get; set; } = null!;

    public bool? HaveDocs { get; set; }

    public string? ReasonOfRelocation { get; set; }

    public string? PanNumber { get; set; }

    public string? TeliPhonicInTime { get; set; }

    public bool? F2favaillability { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
