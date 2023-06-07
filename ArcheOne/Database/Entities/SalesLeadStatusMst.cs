using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class SalesLeadStatusMst
{
    public int Id { get; set; }

    public string SalesLeadStatusName { get; set; } = null!;

    public string SalesLeadStatusCode { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
