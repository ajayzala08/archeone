namespace ArcheOne.Models.Req
{
    public class AddUpdateTaskReqModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string TaskModule { get; set; }
        public string TaskStatus { get; set; }
        public DateTime TaskDate { get; set; }
        public string TimeSpentHH { get; set; }
        public string TimeSpentMM { get; set; }
        public string TaskDescription { get; set; }

        public DateTime? CompletionDate { get; set; } = null;
        public string? TaskName { get; set; }
    }
}
