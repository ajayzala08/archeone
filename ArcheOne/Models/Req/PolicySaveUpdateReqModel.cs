namespace ArcheOne.Models.Req
{
    public class PolicySaveUpdateReqModel
    {
        public int Id { get; set; }
        public string PolicyName { get; set; }
        public IFormFile  PolicyDocumentName { get; set; }
    }
}
