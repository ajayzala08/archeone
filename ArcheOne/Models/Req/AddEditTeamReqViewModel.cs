namespace ArcheOne.Models.Req
{
    public class AddEditTeamReqViewModel
    {
        public int TeamLeadId { get; set; }
        public List<string> TeamMemberIds { get; set; }
        public List<UserDetail> TeamLeadList { get; set; }
        public List<UserDetail> TeamMemberList { get; set; }

        public class UserDetail
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public int RoleId { get; set; }
        }
    }
}
