namespace ArcheOne.Models.Req
{
    public class SaveUpdateSalesLeadReqModel
    {
        public SaveUpdateSalesLeadDetail SaveUpdateSalesLeadDetails { get; set; }
        public List<SaveUpdateSalesLeadContactPersonDetail> SaveUpdateSalesLeadContactPersonList { get; set; }

    }
    public class SaveUpdateSalesLeadDetail
    {
        public int Id { get; set; }
        public string OrgName { get; set; } = null!;

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        public string Address { get; set; } = null!;

        public string Phone1 { get; set; } = null!;

        public string Phone2 { get; set; } = null!;

        public string Email1 { get; set; } = null!;

        public string Email2 { get; set; } = null!;

        public string WebsiteUrl { get; set; } = null!;

        public bool IsActive { get; set; }
    }
    public class SaveUpdateSalesLeadContactPersonDetail
    {
        public int SalesLeadContactPersonId { get; set; }

        public int SalesLeadId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Designation { get; set; }

        public string Mobile1 { get; set; } = null!;

        public string? Mobile2 { get; set; }

        public string Linkedinurl { get; set; } = null!;

        public bool IsActive { get; set; }

    }
}
