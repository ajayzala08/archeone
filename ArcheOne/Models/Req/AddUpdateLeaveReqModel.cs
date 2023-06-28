namespace ArcheOne.Models.Req
{
    public class AddUpdateLeaveReqModel
    {
        public int Id { get; set; }

        public int LeaveTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
        public string Reason { get; set; } = null!;


    }
}
