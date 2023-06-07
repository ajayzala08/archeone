using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Req
{
    public class AddEditTeamReqViewModel
    {
        public int TeamLeadId { get; set; }

        public int TeamMemberId { get; set; }
    }
}
