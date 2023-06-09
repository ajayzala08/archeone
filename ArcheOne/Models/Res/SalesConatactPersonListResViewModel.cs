namespace ArcheOne.Models.Res
{
    public class SalesConatactPersonListResViewModel
    {
        public int Id { get; set; }

        public int SalesLeadId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Designation { get; set; }

        public string Mobile1 { get; set; } = null!;

        public string? Mobile2 { get; set; }

        public string Linkedinurl { get; set; } = null!;
    }
}
