namespace ArcheOne.Models.Res
{
    public class DailyTaskExcelResModel
    {
        public int Id { get; set; }

        public string ProjectName { get; set; } = null!;

        public string ProjectStatus { get; set; } = null!;

        public List<Resources> UserMst { get; set; }
        public List<DailyTask> DailyTaskMsts { get; set; }

        public class Resources
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int RoleId { get; set; }
            public string Role { get; set; }
            public decimal Salary { get; set; }
        }

        public class DailyTask
        {
            public int Id { get; set; }
            public int ProjectId { get; set; }
            public string TimeSpent { get; set; }
            public string TaskModule { get; set; }
            public int CreatedBy { get; set; }
        }
    }
}
