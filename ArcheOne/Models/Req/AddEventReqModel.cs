namespace ArcheOne.Models.Req
{
    public class AddEventReqModel
    {
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string TheamColor { get; set; }
        public bool IsFullDay { get; set; }
        public string EventType { get; set; }

    }
}
