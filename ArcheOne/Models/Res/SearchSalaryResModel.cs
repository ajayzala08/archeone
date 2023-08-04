namespace ArcheOne.Models.Res
{
    public class SearchSalaryResModel
    {
        public bool IsDeletable { get; set; }

        public List<SalaryDetail> SalaryDetails { get; set; }
        public class SalaryDetail
        {
            public int? SalaryId { get; set; }
            public string? EmployeeName { get; set; }
            public int? EmployeeCode { get; set; }
        }
    }
}
