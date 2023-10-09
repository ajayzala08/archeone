using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Res
{
    public class AddEditAppraisalResModel
    {
        public int Id { get; set; }
        public List<UserMst> ReportingManagerId { get; set; }
        public List<UserMst> EmployeeId { get; set; }
        public string Year { get; set; } = null!;

        public ReportingManagetDetail reportingManagetDetail { get; set; }
    }
    public class ReportingManagetDetail
    {
        public int ReportingManagerId { get; set; } = 0;
        public string ReportingManagerName { get; set; }
        public EmployeeDetail EmployeeDetail { get; set; }
    }
    public class EmployeeDetail
    {
        public int EmployeeId { get; set; } = 0;
    }
}
