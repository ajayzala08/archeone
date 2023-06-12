﻿using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class TeamMst
{
    public int Id { get; set; }

    public int TeamLeadId { get; set; }

    public int TeamMemberId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
