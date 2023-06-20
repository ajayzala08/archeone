namespace ArcheOne.Models.Req
{
    public class AddEditPolicyReqModel
    {
        public int Id { get; set; }
        public string PolicyName { get; set; }
        public byte[] PolicyDocumentName { get; set; }
    }
}
