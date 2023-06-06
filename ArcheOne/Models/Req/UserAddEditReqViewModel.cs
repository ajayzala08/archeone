using System.ComponentModel.DataAnnotations;
using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Req
{
	public class UserAddEditReqViewModel
	{
		public List<CompanyMst> CompanyList { get; set; }
		public List<RoleMst> RoleList { get; set; }
		public UserDetail UserDetails { get; set; }
	}
	public class UserDetail
	{
		public int Id { get; set; }
		public int CompanyId { get; set; }
		public string FirstName { get; set; } = null!;
		public string MiddleName { get; set; } = null!;
		public string LastName { get; set; } = null!;
		public string UserName { get; set; } = null!;
		public string Password { get; set; } = null!;
		public string Address { get; set; } = null!;
		public string Pincode { get; set; } = null!;
		public string Mobile1 { get; set; } = null!;
		public string Mobile2 { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string PhotoUrl { get; set; } = null!;
		public int? RoleId { get; set; }
		public bool IsActive { get; set; }
		public bool IsDelete { get; set; }
		public int CreatedBy { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
	}
}
