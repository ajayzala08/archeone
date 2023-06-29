namespace ArcheOne.Models.Req
{
    public class AddUpdateLeaveReqModel
    {
        public int Id { get; set; }

        public int LeaveTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }
        public string Reason { get; set; } = null!;


    }
}
