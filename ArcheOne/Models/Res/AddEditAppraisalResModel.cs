using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Res
{
    public class AddEditAppraisalResModel
    {
        public int Id { get; set; }

        public List<UserMst> EmployeeId { get; set; }

        public List<UserMst> ReportingManagerId { get; set; }

        public string Year { get; set; } = null!;
    }
}
