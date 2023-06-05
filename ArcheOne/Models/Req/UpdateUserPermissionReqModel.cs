namespace ArcheOne.Models.Req
{
    public class UpdateUserPermissionReqModel
    {
        public int UserId { get; set; }
        public List<int> PermissionIds { get; set; }
        public int CreatedBy { get; set; }
    }
}
