using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class RecruitmentMst
{
    public int Id { get; set; }

    public int RecruitmentForId { get; set; }

    public int ClientId { get; set; }

    public string JobCode { get; set; } = null!;

    public string? MainSkill { get; set; }

    public int NoofPosition { get; set; }

    public string Location { get; set; } = null!;

    public string EndClient { get; set; } = null!;

    public int TotalMinExperience { get; set; }

    public int TotalMaxExperience { get; set; }

    public int RelevantMinExperience { get; set; }

    public int RelevantMaxExperience { get; set; }

    public decimal BillRate { get; set; }

    public decimal PayRate { get; set; }

    public int PositionTypeId { get; set; }

    public int RecruitmentTypeId { get; set; }

    public int EmploymentTypeId { get; set; }

    public string Pocname { get; set; } = null!;

    public string MandatorySkills { get; set; } = null!;

    public string JobDescription { get; set; } = null!;

    public int? AssignToid { get; set; }

    public int? Status { get; set; }

    public bool? IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
