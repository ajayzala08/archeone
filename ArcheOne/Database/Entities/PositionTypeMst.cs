using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class PositionTypeMst
{
    public int Id { get; set; }

    public string PositionTypeName { get; set; } = null!;

    public string PositionTypeCode { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
