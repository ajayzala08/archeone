using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Res
{
    public class UserDetailsAddEditResModel
    {
        public List<DepartmentMst> DepartmentList { get; set; }
        public List<EmploymentTypeMst> EmploymentTypeList { get; set; }
        public List<DesignationMst> DesignationList { get; set; }
        public List<UserMst> ReportingManagerList { get; set; }
        public UserDetails UserDetail { get; set; }
    }

    public class UserDetails
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string EmployeeCode { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public string EmergencyContact { get; set; } = null!;

        public string Dob { get; set; } = null!;

        public string PostCode { get; set; } = null!;

        public int EmploymentType { get; set; }

        public int Department { get; set; }

        public int Designation { get; set; }

        public string Location { get; set; } = null!;

        public string BloodGroup { get; set; } = null!;

        public string OfferDate { get; set; } = null!;

        public string JoinDate { get; set; } = null!;

        public string BankName { get; set; } = null!;

        public string AccountNumber { get; set; } = null!;

        public string Branch { get; set; } = null!;

        public string IfscCode { get; set; } = null!;

        public string PfaccountNumber { get; set; } = null!;

        public string PancardNumber { get; set; } = null!;

        public string AdharCardNumber { get; set; } = null!;

        public decimal Salary { get; set; }

        public int ReportingManager { get; set; }

        public string Reason { get; set; } = null!;

        public string EmployeePersonalEmailId { get; set; } = null!;

        public string ProbationPeriod { get; set; } = null!;

        public bool? IsActive { get; set; }
    }
}
