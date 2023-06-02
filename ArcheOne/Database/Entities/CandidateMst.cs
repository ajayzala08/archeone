using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class CandidateMst
{
    public int Id { get; set; }

    public string CandidateName { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string MaritalStatus { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string? AlternateNumber { get; set; }

    public string Email { get; set; } = null!;

    public string? AadharNumber { get; set; }

    public string? PanNumber { get; set; }

    public string? CurrentAddress { get; set; }

    public string? PermanentAddress { get; set; }

    public int? Country { get; set; }

    public string? EmergencyContact { get; set; }

    public string? CompanyName { get; set; }

    public string JoiningLocation { get; set; } = null!;

    public string CurrentDesignation { get; set; } = null!;

    public string OfferDesignation { get; set; } = null!;

    public int? TotalExperience { get; set; }

    public int? RelevantExperience { get; set; }

    public string Skill { get; set; } = null!;

    public DateTime? SelectionDate { get; set; }

    public DateTime? OfferDate { get; set; }

    public DateTime? JoiningDate { get; set; }

    public int Ctc { get; set; }

    public int Ectc { get; set; }

    public int? MarginPercentage { get; set; }

    public int? Gp { get; set; }

    public int? EmploymentTypeId { get; set; }

    public int? InterviewStatusId { get; set; }

    public int? BillRate { get; set; }

    public int? PayRate { get; set; }

    public string? BankAccountNo { get; set; }

    public string? BanlName { get; set; }

    public string? Branch { get; set; }

    public string? Ifsccode { get; set; }

    public string? Note { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
