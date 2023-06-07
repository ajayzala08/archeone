namespace ArcheOne.Models.Res
{
    public class SalesLeadListResViewModel
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
    }
}
