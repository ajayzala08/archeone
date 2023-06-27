namespace ArcheOne.Models.Req
{
    public class AppraisalRatingSaveUpdateReqModel
    {
        public int Id { get; set; }
        public int ReportingManagerId { get; set; }
        public int EmployeeId { get; set; }
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
