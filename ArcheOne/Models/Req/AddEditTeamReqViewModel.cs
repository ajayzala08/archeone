using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Req
{
    public class AddEditTeamReqViewModel
    {
        public List<UserMst> TeamLeadList { get; set; }

        public List<UserMst> TeamMemberList { get; set; }
        public TeamLeadDetails TeamLeadDetails { get; set; }

    }
    public class TeamLeadDetails
    {
        public int TeamLeadId { get; set; }
        public string TeamLeadName { get; set; }
        //public string TeamMemberName { get; set; }

        //public int TeamLeadId { get; set; }
        //public int TeamMemberId { get; set; }

        public List<TeamMemberDetails> TeamMemberDetails { get; set; } 

    }
    public class TeamMemberDetails
    {
        public int TeamMemberId { get; set; }
        public string TeamMemberName { get; set; }

    }
}
