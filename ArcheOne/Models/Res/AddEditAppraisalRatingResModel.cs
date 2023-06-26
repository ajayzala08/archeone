using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Res
{
    public class AddEditAppraisalRatingResModel
    {
        public int Id { get; set; }
        public List<UserMst> ReportingManagerId { get; set; }
        public List<UserMst> EmployeeId { get; set; }
        public ManagetDetail reportingManagetDetail { get; set; }
        public string Date { get; set; }
        public string ReviewDate { get; set; }

        public bool IsUserHR { get; set; }
        public bool IsUserEmployee { get; set; }
        public bool IsUserReportManager { get; set; }

        public EmployeeRating EmployeeRating { get; set; }

        public AppraisalRating appraisalRating { get; set; }

 
    }
    public class ManagetDetail
    {
        public int ReportingManagerId { get; set; } = 0;
        public EmployeesDetail EmployeeDetail { get; set; }
    }
    public class EmployeesDetail
    {
        public int EmployeeId { get; set; } = 0;
    }
    public class AppraisalRating
    {
        public int QualityOfWork { get; set; }

        public int GoalNtarget { get; set; }

        public int WrittenVerbalSkill { get; set; }

        public int InitiativeMotivation { get; set; }

        public int? TeamWork { get; set; }

        public int? ProblemSolvingAbillity { get; set; }

        public int? Attendance { get; set; }

        public int? Total { get; set; }

        public string? Comment { get; set; }


    }
    public class EmployeeRating
    {
        public int QualityOfWork { get; set; }

        public int GoalNtarget { get; set; }

        public int WrittenVerbalSkill { get; set; }

        public int InitiativeMotivation { get; set; }

        public int? TeamWork { get; set; }

        public int? ProblemSolvingAbillity { get; set; }

        public int? Attendance { get; set; }

        public int? Total { get; set; }

        public string? Comment { get; set; }
    }

}

