namespace ArcheOne.Models.Req
{
    public class ResetPasswordReqModel
    {
        public string UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
