namespace ArcheOne.Models.Req
{
	public class RoleReqModel
	{
		public int Id { get; set; }

		public string RoleName { get; set; } = null!;

		public string RoleCode { get; set; } = null!;

		public bool IsActive { get; set; }

		public bool IsDelete { get; set; }

		public int CreatedBy { get; set; }

		public int UpdatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public DateTime UpdatedDate { get; set; }
	}
}
