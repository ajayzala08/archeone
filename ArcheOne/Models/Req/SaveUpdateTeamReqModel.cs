namespace ArcheOne.Models.Req
{
    public class SaveUpdateTeamReqModel
    {
        //public int TeamId { get; set; }
        public int TeamLeadId { get; set; }
        public List<int> TeamMemberId { get; set; }

    }
}
