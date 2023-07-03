using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class EventMst
{
    public int Id { get; set; }

    public string? Subject { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? ThemeColour { get; set; }

    public string? EventType { get; set; }

    public bool? IsFullDay { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
