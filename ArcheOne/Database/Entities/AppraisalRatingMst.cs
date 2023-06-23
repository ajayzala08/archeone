using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class AppraisalRatingMst
{
    public int Id { get; set; }

    public int RatingFromUserId { get; set; }

    public int RatingToUserId { get; set; }

    public int QualityOfWork { get; set; }

    public int GoalNtarget { get; set; }

    public int WrittenVerbalSkill { get; set; }

    public int InitiativeMotivation { get; set; }

    public int? TeamWork { get; set; }

    public int? ProblemSolvingAbillity { get; set; }

    public int? Attendance { get; set; }

    public int? Total { get; set; }

    public string? Comment { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
