namespace ArcheOne.Models.Req
{
    public class UpdateDefaultPermissionReqModel
    {
        public int RoleId { get; set; }
        public List<int> PermissionIds { get; set; }
        public int CreatedBy { get; set; }
        public bool UpdateRoleWithUsers { get; set; }
    }
}
