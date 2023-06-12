using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Req
{
    public class AddEditTeamReqViewModel
    {
        public List<UserMst> TeamLeadList { get; set; }

        public List<UserMst> TeamMemberList { get; set; }
        public TeamDetails TeamDetails { get; set; }

    }
    public class TeamDetails
    {
        public int TeamId { get; set; } 
        public string TeamLeadId { get; set; }

        public string TeamMemberId { get; set; }

        public int TeamLeadId1 { get; set; }
        public int TeamMemberId1 { get; set; }

    }
}
