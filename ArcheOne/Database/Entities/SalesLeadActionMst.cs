using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class SalesLeadActionMst
{
    public int Id { get; set; }

    public string SalesLeadActionName { get; set; } = null!;

    public string SalesLeadActionCode { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
