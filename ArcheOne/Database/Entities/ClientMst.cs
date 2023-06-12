using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class ClientMst
{
    public int Id { get; set; }

    public string ClientName { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public int CompanyId { get; set; }

    public string EmailId { get; set; } = null!;

    public string MobileNo { get; set; } = null!;

    public string? PocnamePrimary { get; set; }

    public string? PocnumberPrimary { get; set; }

    public string? PocemailPrimary { get; set; }

    public string? PocnameSecondary { get; set; }

    public string? PocnumberSecondary { get; set; }

    public string? PocemailSecondary { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
