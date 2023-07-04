namespace ArcheOne.Models.Res
{
    public class GetPolicyListResModel
    {
        public int Id { get; set; }
        public string PolicyName { get; set; }
        public string PolicyDocument { get; set; }
        public bool? IsUserHR { get; set; }
    }
}
