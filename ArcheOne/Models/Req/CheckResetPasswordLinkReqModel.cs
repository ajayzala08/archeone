namespace ArcheOne.Models.Req
{
    public class CheckResetPasswordLinkReqModel
    {
        public string Id { get; set; }
        public string Link { get; set; }
        public string SecurityCode { get; set; }
    }
}
