using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class DailyTaskMst
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public DateTime TaskDate { get; set; }

    public string TaskStatus { get; set; } = null!;

    public string TimeSpent { get; set; } = null!;

    public string TaskModule { get; set; } = null!;

    public string TaskDescription { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
