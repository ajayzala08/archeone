namespace ArcheOne.Models.Req
{
    public class SaveUpdateTeam
    {
        public int TeamId { get; set; }
        public int TeamLeadId { get; set; }

        //public List<int> TeamMemberId { get; set; }
        public int TeamMemberId { get; set; }

        //public bool IsActive { get; set; }

    }
}
