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
   
        public string TeamLead { get; set; }
        public string TeamMemberName { get; set; }

        public int TeamLeadId { get; set; }
        public int TeamMemberId { get; set; }

    }
}
