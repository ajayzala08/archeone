using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class SalesContactPersonMst
{
    public int Id { get; set; }

    public int SalesLeadId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Designation { get; set; }

    public string Mobile1 { get; set; } = null!;

    public string? Mobile2 { get; set; }

    public string Linkedinurl { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
