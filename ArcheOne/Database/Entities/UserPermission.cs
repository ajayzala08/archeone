﻿using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class UserPermission
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PermissionId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}