namespace ArcheOne.Models.Req
{
	public class AddEditUserDetailsReqModel
	{
		public int Id { get; set; }
		public int UserId { get; set; }

		public string EmployeeCode { get; set; } = null!;

		public string Gender { get; set; } = null!;

		public string EmergencyContact { get; set; } = null!;

		public DateTime Dob { get; set; } 

		public string PostCode { get; set; } = null!;

		public int EmploymentType { get; set; }

		public int Department { get; set; }

		public int Designation { get; set; }

		public string Location { get; set; } = null!;

		public string BloodGroup { get; set; } = null!;

		public DateTime OfferDate { get; set; }

		public DateTime JoinDate { get; set; }

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
	}
}
