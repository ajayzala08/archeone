using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class RequirementMst
{
    public int Id { get; set; }

    public int RequirementForId { get; set; }

    public int ClientId { get; set; }

    public string JobCode { get; set; } = null!;

    public string? MainSkill { get; set; }

    public int NoOfPosition { get; set; }

    public string Location { get; set; } = null!;

    public string EndClient { get; set; } = null!;

    public decimal TotalMinExperience { get; set; }

    public decimal TotalMaxExperience { get; set; }

    public decimal RelevantMinExperience { get; set; }

    public decimal RelevantMaxExperience { get; set; }

    public decimal ClientBillRate { get; set; }

    public decimal CandidatePayRate { get; set; }

    public int PositionTypeId { get; set; }

    public int RequirementTypeId { get; set; }

    public int EmploymentTypeId { get; set; }

    public string Pocname { get; set; } = null!;

    public string MandatorySkills { get; set; } = null!;

    public string? JobDescription { get; set; }

    public string? AssignedUserIds { get; set; }

    public int RequirementStatusId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
