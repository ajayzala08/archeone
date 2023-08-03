namespace ArcheOne.Models.Res
{
    public class GetPolicyListResModel
    {
        public bool IsEditable { get; set; }
        public bool IsDeletable { get; set; }
        public List<PolicyDetail> PolicyDetails { get; set; }

        public class PolicyDetail
        {
            public int Id { get; set; }
            public string PolicyName { get; set; }
            public string PolicyDocument { get; set; }
            public bool? IsUserHR { get; set; }
        }
    }
}
