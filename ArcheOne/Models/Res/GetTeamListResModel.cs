namespace ArcheOne.Models.Res
{
    public class GetTeamListResModel
    {
        public int Id { get; set; }
        public string TeamLead { get; set; }
        public string TeamName { get; set; }
        public int TeamLeadId { get; set; }
        public int TeamMemberId { get; set; }
    }
}
