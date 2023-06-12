namespace ArcheOne.Models.Res
{
	public class UserListModel
	{
		public int Id { get; set; }

		public string CompanyId { get; set; }

		public string FirstName { get; set; }

		public string MiddleName { get; set; } 

		public string LastName { get; set; } 

		public string UserName { get; set; }

		public string Password { get; set; }

		public string Address { get; set; }

		public string Pincode { get; set; }

		public string Mobile1 { get; set; }

		public string Mobile2 { get; set; }

		public string Email { get; set; }

		public string PhotoUrl { get; set; }

		public bool? IsActive { get; set; }

		public bool IsDelete { get; set; }

		public int CreatedBy { get; set; }

		public int UpdatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public DateTime UpdatedDate { get; set; }

		public string RoleId { get; set; }
	}
}
