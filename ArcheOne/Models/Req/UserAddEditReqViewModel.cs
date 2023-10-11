using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Req
{
    public class UserAddEditReqViewModel
    {
        public List<DesignationMst> DesignationaddList { get; set; }
        public List<RoleMst> RoleList { get; set; }
        public List<DepartmentMst> DepartmentList { get; set; }
        public List<DesignationDetails> DesignationList { get; set; }
        public UserDetail UserDetails { get; set; }
    }
    public class DesignationDetails
    {
        public int Id { get; set; }
        public string Designation { get; set; }
        public int RoleId { get; set; }
        public int DepartmentId { get; set; }
    }
    public class UserDetail
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Pincode { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public int RoleId { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public bool IsActive { get; set; }
    }
}
