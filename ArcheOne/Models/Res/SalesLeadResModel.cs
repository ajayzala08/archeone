namespace ArcheOne.Models.Res
{
	public class SalesLeadResModel
	{
		public int Id { get; set; }
		public string OrgName { get; set; }
		public int ContactPersonId { get; set; }
		public string FullName { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public string Designation { get; set; }
		public string? LeadStatus { get; set; }

	}
}
