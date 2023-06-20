namespace ArcheOne.Models.Res
{
    public class ProfileResModel
    {
        public int UserId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DOB { get; set; } = string.Empty;
        public string DOJ { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
    }
}
