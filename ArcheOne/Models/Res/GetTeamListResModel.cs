namespace ArcheOne.Models.Res
{
    public class GetTeamListResModel
    {
        public bool IsEditable { get; set; }
        public bool IsDeletable { get; set; }
        public List<TeamDetail> TeamDetails { get; set; }
        public class TeamDetail
        {
            public int Id { get; set; }
            public int TeamLeadId { get; set; }
            public string TeamName { get; set; }
            public string TeamLeadName { get; set; }
            public string TeamMemeberIds { get; set; }
            public string TeamMemebersNames { get; set; }
        }
    }
}
