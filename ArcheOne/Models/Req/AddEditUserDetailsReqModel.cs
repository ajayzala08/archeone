﻿namespace ArcheOne.Models.Req
{
	public class AddEditUserDetailsReqModel
	{
		//public int UserId { get; set; }

		public string EmployeeCode { get; set; } = null!;

		public string Gender { get; set; } = null!;

		public string EmergencyContact { get; set; } = null!;

		public DateTime Dob { get; set; }

		public string PostCode { get; set; } = null!;

		public string EmploymentType { get; set; } = null!;

		public string Department { get; set; } = null!;

		public string Designation { get; set; } = null!;

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

		public string ReportingManager { get; set; } = null!;

		public string Reason { get; set; } = null!;

		public string EmployeePersonalEmailId { get; set; } = null!;

		public string ProbationPeriod { get; set; } = null!;
	}
}