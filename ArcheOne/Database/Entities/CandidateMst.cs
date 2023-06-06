using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class CandidateMst
{
    public int Id { get; set; }

    public int InterviewId { get; set; }

    public string FirstName { get; set; } = null!;

    public string MiddleName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string MaritalStatus { get; set; } = null!;

    public string Mobile1 { get; set; } = null!;

    public string? Mobile2 { get; set; }

    public string Email1 { get; set; } = null!;

    public string Email2 { get; set; } = null!;

    public string? AadharNumber { get; set; }

    public string? PanNumber { get; set; }

    public string? CurrentAddress { get; set; }

    public string? PermanentAddress { get; set; }

    public int CountryId { get; set; }

    public int StateId { get; set; }

    public int CityId { get; set; }

    public string? EmergencyContact { get; set; }

    public string? EndClient { get; set; }

    public string JoiningLocation { get; set; } = null!;

    public string CurrentDesignation { get; set; } = null!;

    public string OfferDesignation { get; set; } = null!;

    public decimal TotalExperience { get; set; }

    public decimal RelevantExperience { get; set; }

    public string Skill { get; set; } = null!;

    public DateTime SelectionDate { get; set; }

    public DateTime OfferDate { get; set; }

    public DateTime JoiningDate { get; set; }

    public decimal Ctc { get; set; }

    public decimal Ectc { get; set; }

    public decimal MarginPercentage { get; set; }

    public decimal Gp { get; set; }

    public int? ClientBillRate { get; set; }

    public int? CandidatePayRate { get; set; }

    public int EmploymentTypeId { get; set; }

    public int HireStatusId { get; set; }

    public string? BankAccountNo { get; set; }

    public string? BankName { get; set; }

    public string? BankBranch { get; set; }

    public string? Ifsccode { get; set; }

    public string? Note { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
