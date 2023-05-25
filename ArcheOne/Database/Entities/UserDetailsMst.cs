using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class UserDetailsMst
{
    public int Id { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string MiddleName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string EmergencyContact { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string ConfirmPassword { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
