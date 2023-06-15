﻿using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class HolidayMst
{
    public int Id { get; set; }

    public string HolidayName { get; set; } = null!;

    public DateTime HolidayDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
