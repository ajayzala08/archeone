namespace ArcheOne.Models.Res
{
    public class GetAppraisalListResModel
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string ReportingManagerName { get; set; }
        public string Year { get; set; }
        public bool IsUserHR { get; set; }

    }

}
