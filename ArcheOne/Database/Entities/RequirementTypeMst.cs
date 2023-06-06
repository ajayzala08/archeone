﻿using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class RequirementTypeMst
{
    public int Id { get; set; }

    public string RequirementTypeName { get; set; } = null!;

    public string RequirementTypeCode { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
