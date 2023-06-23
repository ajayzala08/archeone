using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class AppraisalMst
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int ReportingManagerId { get; set; }

    public string Year { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
