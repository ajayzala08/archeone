using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class LinkMst
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ResetPasswordLink { get; set; } = null!;

    public bool IsClicked { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ExpiredDate { get; set; }
}
