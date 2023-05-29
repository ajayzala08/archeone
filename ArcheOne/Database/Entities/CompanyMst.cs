using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class CompanyMst
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Pincode { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Mobile1 { get; set; } = null!;

    public string Mobile2 { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Website { get; set; } = null!;

    public string LogoUrl { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
