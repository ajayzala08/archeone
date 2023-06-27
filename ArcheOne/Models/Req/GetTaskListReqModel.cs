namespace ArcheOne.Models.Req
{
    public class GetTaskListReqModel
    {
        public int ProjectId { get; set; } = 0;
        public int ResourceId { get; set; } = 0;
        public DateTime? FromDate { get; set; } = null;
        public DateTime? ToDate { get; set; } = null;
    }
}
