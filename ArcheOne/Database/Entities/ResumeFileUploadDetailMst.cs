using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class ResumeFileUploadDetailMst
{
    public int Id { get; set; }

    public int ResumeFileUploadId { get; set; }

    public string FullName { get; set; } = null!;

    public string Mobile1 { get; set; } = null!;

    public string? Mobile2 { get; set; }

    public string? Mobile3 { get; set; }

    public string Email1 { get; set; } = null!;

    public string? Email2 { get; set; }

    public decimal TotalExperienceAnnual { get; set; }

    public decimal RelevantExperienceYear { get; set; }

    public string HighestQualification { get; set; } = null!;

    public string? GapReason { get; set; }

    public string? CurrentEmployer { get; set; }

    public string? CurrentDesignation { get; set; }

    public decimal CurrentCtcAnnual { get; set; }

    public decimal CurrentTakeHomeMonthly { get; set; }

    public bool CurrentPfdeduction { get; set; }

    public decimal ExpectedCtcAnnual { get; set; }

    public decimal ExpectedTakeHomeMonthly { get; set; }

    public bool ExpectedPfdeduction { get; set; }

    public string? LastSalaryHike { get; set; }

    public string? SalaryHikeReason { get; set; }

    public decimal NoticePeriodDays { get; set; }

    public decimal ExpectedJoinInDays { get; set; }

    public string? ReasonForEarlyJoin { get; set; }

    public bool OfferInHand { get; set; }

    public string? OfferInHandReason { get; set; }

    public bool HasAllDocuments { get; set; }

    public string CurrentLocation { get; set; } = null!;

    public string WorkLocation { get; set; } = null!;

    public string? ReasonForRelocation { get; set; }

    public string NativePlace { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string Pan { get; set; } = null!;

    public DateTime? TeleInterviewTime { get; set; }

    public bool F2favailability { get; set; }

    public DateTime? F2finterviewTime { get; set; }

    public string? Skills { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public DateTime? JoinInDate { get; set; }

    public decimal? OfferedPackageInLac { get; set; }

    public string? JoinInNote { get; set; }
}
