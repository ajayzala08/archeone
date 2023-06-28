namespace ArcheOne.Models.Res
{
    public class SalesLeadAddEditResModel
    {
        public SalesLeadDetail salesLeadDetail { get; set; }
        public List<SalesLeadContactPersonDetail> ContactPersonDetailList { get; set; }
    }

    public class SalesLeadDetail
    {
        public int SalesLeadId { get; set; }
        public string OrgName { get; set; } 
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; } 
        public string Phone1 { get; set; } 
        public string Phone2 { get; set; } 
        public string Email1 { get; set; }
        public string Email2 { get; set; } 
        public string WebsiteUrl { get; set; } 
        public bool IsActive { get; set; }
    }

    public class SalesLeadContactPersonDetail
    {
        public int SalesLeadContactPersonId { get; set; }
        public int SalesLeadId { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string Email { get; set; } 
        public string? Designation { get; set; }
        public string Mobile1 { get; set; }
        public string? Mobile2 { get; set; }
        public string Linkedinurl { get; set; }
        public bool IsActive { get; set; }
    }
}

