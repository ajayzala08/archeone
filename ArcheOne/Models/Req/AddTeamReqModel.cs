namespace ArcheOne.Models.Req
{
    public class AddTeamReqModel
    {
        public int TeamLeadId { get; set; }

        public int TeamMemberId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDelete { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}

