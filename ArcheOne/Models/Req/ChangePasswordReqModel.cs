namespace ArcheOne.Models.Req
{
    public class ChangePasswordReqModel
    {
        public string UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
