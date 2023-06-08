using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Req
{
    public class AddEditTeamReqViewModel
    {
        public List<UserMst> TeamLeadList { get; set; }

        public List<UserMst> TeamMemberList { get; set; }
        public int TeamId { get; set; } = 0;
        public TeamDetails TeamDetails { get; set; }

    }
    public class TeamDetails
    {
        public int TeamId { get; set; } = 0;
        public int TeamLeadId { get; set; }

        public int TeamMemberId { get; set; }
    }
}
