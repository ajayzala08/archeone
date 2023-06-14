namespace ArcheOne.Models.Req
{
    public class ScheduleInterviewReqModel
    {
        public int ResumeFileUploadId { get; set; }
        public int InterviewRoundId { get; set; }
        public int ResumeFileUploadDetailId { get; set; }
        public int InterviewRoundTypeId { get; set; } = 0;
        public DateTime InterviewStartDateTime { get; set; }
        public string InterviewBy { get; set; }
        public string? InterviewLocation { get; set; }
        public string? Note { get; set; }
    }
}
